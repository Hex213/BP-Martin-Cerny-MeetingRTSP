using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MeetingClientWPF.Hex
{
    public enum ErrorCode
    {
        //Vseobecne
        Null,
        BadFormat,
        //Network
        PingError
    };

    public static class HexConnectionCheck
    {
        private static ErrorCode _errorCode;

        public static ErrorCode LastErrorCode => _errorCode;

        public static bool IsActive(in string IP, in ushort timeout, out long time)
        {
            IPAddress ipAddress;
            time = -1;

            try
            {
                ipAddress = IPAddress.Parse(IP);
            }
            catch (ArgumentNullException e)
            {
                _errorCode = ErrorCode.Null;
                return false;
            }
            catch (FormatException e)
            {
                _errorCode = ErrorCode.BadFormat;
                return false;
            }
            

            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            options.DontFragment = true;

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            PingReply reply;

            if (ipAddress.ToString().Equals(IPAddress.Loopback.ToString()))
            {
                return true;
            }

            try
            {
                reply = pingSender.Send(ipAddress, timeout, buffer, options);
            }
            catch (PingException e)
            {
                _errorCode = ErrorCode.PingError;
                return false;
            }
            catch (Exception)
            {
                _errorCode = ErrorCode.PingError;
                return false;
            }
            if (reply.Status == IPStatus.Success)
            {
                time = reply.RoundtripTime;
            }

            return true;
        }
    }
}
