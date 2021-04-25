using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CSNamedPipe;
using LibHexUtils.Arrays;

namespace LibRtspClientSharp.Hex
{
    public static class IPC
    {
        private static Process proc;
        private static NamedPipeServer Pin;
        private static NamedPipeServer Pout;

        public static bool IsStarted()
        {
            return proc.Id != 0 && !proc.HasExited;
        }

        private static void RunHosting(string name)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            process.StartInfo.FileName = "MeetingServer.exe";
            //process.StartInfo.FileName = "cmd.exe";
            //process.StartInfo.RedirectStandardInput = true;
            //process.StartInfo.RedirectStandardOutput = true;
            //process.StartInfo.CreateNoWindow = true;
            //process.StartInfo.UseShellExecute = false;
            //process.StartInfo.Arguments = "/C MeetingServer.exe " + name;
            process.StartInfo.Arguments = name;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            var started = process.Start();
            if (process.Id != 0 && !process.HasExited && started)
            {
                proc = process;
            }

            //process.WaitForExit();// Waits here for the process to exit.
        }

        public static void Send(byte[] data, int startOff = 0, int count = 0)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (startOff < 0) throw new ArgumentOutOfRangeException(nameof(startOff));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var send = startOff == 0 && count == 0 ? data : ByteArray.SubArray(data, startOff, count);
            Pout.SendMessage(send, Pout.clientse);
        }

        public static void Start(string name, Func<object, object> readFunc)
        {
            Console.WriteLine(@"\\.\pipe\" + name + "in");
            Pin = new NamedPipeServer(@"\\.\pipe\"+name+"in", 0, readFunc);
            Console.WriteLine(@"\\.\pipe\" + name + "out");
            Pout = new NamedPipeServer(@"\\.\pipe\" + name + "out", 1, null);

            Pin.Start();
            Pout.Start();

            RunHosting(name);
        }
    }
}
