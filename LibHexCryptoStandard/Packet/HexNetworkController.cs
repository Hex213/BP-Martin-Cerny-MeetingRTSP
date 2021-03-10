﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LibHexCryptoStandard.Packet
{
    public static class HexNetworkController
    {
        public static async Task<int> decrypt(byte[] readBuffer, int read, int offset, bool decryptData, bool useBase64, bool output = false)
        {
            if (output) Console.WriteLine("Recv(" + read + ")");

            HexPacket hexPacket = null;

            if (!decryptData)
            {
                return read;
            }

            //todo: nastava chyba
            //1. get packet
            hexPacket = HexPacket.GetPacketFromArr(readBuffer, (ushort)offset, useBase64);
            //2. decrypt
            Object decrypted = hexPacket.Decrypt();
            byte[] toCopy = (byte[])decrypted;
            //3. copy back to buffer
            Buffer.BlockCopy(toCopy, 0, readBuffer, offset, (int)hexPacket.DecryptedBytesSize);
            //4. return of readed
            return (int)hexPacket.DecryptedBytesSize;
        }

        public static async Task<int> Read(Stream stream, byte[] readBuffer, int offset, bool decryptData, bool useBase64, bool output = false, int count = -1)
        {
            //0. read data
            var read = await stream.ReadAsync(readBuffer, offset, (count == -1 ? readBuffer.Length : count));

            //1.decrypt
            return await decrypt(readBuffer, read, offset, decryptData, useBase64, output);
        }

        public static async Task<byte[]> Write(Stream outStream, byte[] buffer, int offset, int count)
        {
            return null;//outStream.WriteAsync(buffer, offset, count);
        }
    }
}