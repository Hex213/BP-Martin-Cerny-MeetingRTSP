using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibHexCryptoStandard.Algoritm;
using LibHexCryptoStandard.Packet;
using LibRtspClientSharp.Hex;
using RtspClientSharp.Rtsp.Authentication;
using RtspClientSharp.Utils;

namespace RtspClientSharp.Rtsp
{
    abstract class RtspTransportClient : IRtspTransportClient
    {
        protected readonly ConnectionParameters ConnectionParameters;
        private readonly byte[] _buffer = new byte[Constants.MaxResponseHeadersSize];

        private Authenticator _authenticator;

        public abstract EndPoint RemoteEndPoint { get; }

        protected RtspTransportClient(ConnectionParameters connectionParameters)
        {
            ConnectionParameters =
                connectionParameters ?? throw new ArgumentNullException(nameof(connectionParameters));
        }

        public abstract Task ConnectAsync(CancellationToken token);

        public abstract Stream GetStream();

        public async Task<RtspResponseMessage> EnsureExecuteRequest(RtspRequestMessage requestMessage,
            CancellationToken token, int responseReadPortionSize = 0)
        {
            RtspResponseMessage responseMessage = await ExecuteRequest(requestMessage, token, responseReadPortionSize);

            if (responseMessage.StatusCode != RtspStatusCode.Ok)
                throw new RtspBadResponseCodeException(responseMessage.StatusCode);

            return responseMessage;
        }

        public async Task<RtspResponseMessage> ExecuteRequest(RtspRequestMessage requestMessage,
            CancellationToken token, int responseReadPortionSize = 0)
        {
            token.ThrowIfCancellationRequested();

            await SendRequestAsync(requestMessage, token);

            RtspResponseMessage responseMessage = await GetResponseAsync(responseReadPortionSize);

            if (responseMessage.StatusCode != RtspStatusCode.Unauthorized)
                return responseMessage;

            if (ConnectionParameters.Credentials.IsEmpty() || _authenticator != null)
                throw new RtspBadResponseCodeException(responseMessage.StatusCode);

            string authenticateHeader = responseMessage.Headers[WellKnownHeaders.WwwAuthenticate];

            if (string.IsNullOrEmpty(authenticateHeader))
                throw new RtspBadResponseCodeException(responseMessage.StatusCode);

            _authenticator = Authenticator.Create(ConnectionParameters.Credentials, authenticateHeader);
            requestMessage.UpdateSequenceNumber();

            await SendRequestAsync(requestMessage, token);
            responseMessage = await GetResponseAsync();

            if (responseMessage.StatusCode == RtspStatusCode.Unauthorized)
                throw new RtspBadResponseCodeException(responseMessage.StatusCode);

            return responseMessage;
        }

        //todo:sennd connection
        public Task SendRequestAsync(RtspRequestMessage requestMessage, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (_authenticator != null)
                AddAuthorizationHeader(requestMessage);

            string requestMessageString = requestMessage.ToString();
            
            int written = Encoding.ASCII.GetBytes(requestMessageString, 0, requestMessageString.Length, _buffer, 0);
            return WriteAsync(_buffer, 0, written);//TOdo:send
        }

        public abstract void Dispose();

        protected abstract Task WriteAsync(byte[] buffer, int offset, int count);
        protected abstract Task<int> ReadAsync(byte[] buffer, int offset, int count);
        protected abstract Task ReadExactAsync(byte[] buffer, int offset, int count);
        
        private async Task<RtspResponseMessage> GetResponseAsync(int responseReadPortionSize = 0)
        {
            int totalRead = await ReadUntilEndOfHeadersAsync(responseReadPortionSize);

            int startOfResponse = ArrayUtils.IndexOfBytes(_buffer, Constants.RtspProtocolNameBytes, 0, totalRead);

            if (startOfResponse == -1)
                throw new RtspBadResponseException("\"RTSP\" start signature is not found");

            int endOfResponseHeaders = ArrayUtils.IndexOfBytes(_buffer, Constants.DoubleCrlfBytes, 0, totalRead) +
                                       Constants.DoubleCrlfBytes.Length;

            if (endOfResponseHeaders == -1)
                throw new RtspBadResponseException("End of response headers is not found");

            var headersByteSegment =
                new ArraySegment<byte>(_buffer, startOfResponse, endOfResponseHeaders - startOfResponse);
            RtspResponseMessage rtspResponseMessage = RtspResponseMessage.Parse(headersByteSegment);

            string contentLengthString = rtspResponseMessage.Headers[WellKnownHeaders.ContentLength];

            if (string.IsNullOrEmpty(contentLengthString))
                return rtspResponseMessage;

            if (!uint.TryParse(contentLengthString, out uint contentLength))
                throw new RtspParseResponseException($"Invalid content-length header: {contentLengthString}");

            if (contentLength == 0)
                return rtspResponseMessage;

            if (contentLength > Constants.MaxResponseHeadersSize)
                throw new RtspBadResponseException($"Response content is too large: {contentLength}");

            int dataPartSize = totalRead - headersByteSegment.Count;

            Buffer.BlockCopy(_buffer, endOfResponseHeaders, _buffer, 0, dataPartSize);
            await ReadExactAsync(_buffer, dataPartSize, (int) (contentLength - dataPartSize));

            rtspResponseMessage.ResponseBody = new ArraySegment<byte>(_buffer, 0, (int) contentLength);
            return rtspResponseMessage;
        }

        //switch decrypt --------------------------------------------------------------------------
        private ushort len_bytes = 4;
        
        private void correctRead(ref int read, ref int totalRead, ref int offset, in int decrypted)
        {
            if(read > 1)
            {
                totalRead -= read;
                read = decrypted;
                totalRead += read;
            }
            else
            {
                read = decrypted;
                totalRead = read;
                offset = totalRead;
            }
        }

        private async Task<int> ReadUntilEndOfHeadersAsync(int readPortionSize = 0)
        {
            int offset = 0;

            bool readNew = true;
            uint readNeeded = 0;

            int endOfHeaders;
            int totalRead = 0;

            do
            {
                int count = _buffer.Length - totalRead;

                if (readPortionSize != 0 && count > readPortionSize)
                    count = readPortionSize;

                if (count == 0)
                    throw new RtspBadResponseException(
                        $"Response is too large (> {Constants.MaxResponseHeadersSize / 1024} KB)");
                
                int read = await ReadAsync(_buffer, offset, count);

                //kontrola existencie streamu
                if (read == 0)
                    throw new EndOfStreamException("End of rtsp stream");

                totalRead += read;

                //Vypis prijatych dat
                if(!Global.strictPrint)
                {
                    Console.WriteLine("Recv(" + read + ")");
                }

                //Ziskanie velkosti Hpaketu
                if ((read > 8 || totalRead > 8) && readNew && ConnectionParameters.Enryption)
                {
                    //todo:posun offset
                    readNeeded = HexPacket.GetSizeFrom(_buffer, 0, len_bytes);

                    if (readNeeded != 0)
                    {
                        readNew = false;
                    }
                }

                var pktSize = totalRead - HexPacket.GetNull().Length - len_bytes;

                //Desifrovanie
                if ((pktSize == readNeeded/* || totalRead == readNeeded*/) && ConnectionParameters.Enryption && !readNew)
                {

                    //var readedAfter = await HexNetworkController.decrypt(_buffer, read, 0,
                    //    ConnectionParameters.Enryption, ConnectionParameters.UseBase64, Global.strictPrint);

                    //if (readedAfter != read)
                    //{
                    //    read = readedAfter;
                    //}
                    //Console.WriteLine("RecvTotal(" + pktSize + ")");

                    byte[] toDecrypt = new byte[totalRead];
                    Buffer.BlockCopy(_buffer, 0, toDecrypt, 0, totalRead);
                    HexPacket hexPacket = new HexPacket(toDecrypt, ConnectionParameters.UseBase64, EncryptType.Decrypt);
                    byte[] decrypted = null;
                    //decrypted
                    decrypted = (byte[])hexPacket.Decrypt();
                    //clear default buffer
                    Array.Clear(_buffer, 0, totalRead);
                    //Copy back to buffer
                    Buffer.BlockCopy(decrypted, 0, _buffer, 0, decrypted.Length);
                    //Correct read sizes
                    correctRead(ref read, ref totalRead, ref offset, decrypted.Length);
                }

                int startIndex = offset - (Constants.DoubleCrlfBytes.Length);

                if (startIndex < 0)
                    startIndex = 0;
                
                endOfHeaders = ArrayUtils.IndexOfBytes(_buffer, Constants.DoubleCrlfBytes, startIndex,
                    totalRead - startIndex);

                offset += read;
            } while (endOfHeaders == -1);
            return totalRead;
        }

        private void AddAuthorizationHeader(RtspRequestMessage request)
        {
            Uri uri = ConnectionParameters.GetFixedRtspUri();

            string headerValue = _authenticator.GetResponse(request.CSeq, uri.ToString(),
                request.Method.ToString(), Array.Empty<byte>());

            request.Headers.Add("Authorization", headerValue);
        }
    }
}