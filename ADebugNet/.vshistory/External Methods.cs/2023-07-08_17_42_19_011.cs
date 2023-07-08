
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ADebugNet
{
    public static class CProcess
    {
        public static void ScanProc()
        {
            Scanner.ScanAndKill();

            if (Debug1.PerformChecks() == 1)
            {
                //Environment.FailFast("");
            }

            if (Debug2.PerformChecks() == 1)
            {
                //Environment.FailFast("");
            }

            Debugt3.HideOSThreads();
        }


        public static void Worker()
        {
            while (true)
            {
                Scanner.ScanAndKill();

                if (Debug1.PerformChecks() == 1)
                {
                    //File.Create(@"C:\Windows\Temp\win64debug.tmp");
                    Thread.Sleep(2000);
                    SelfDestruct(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                }

                if (Debug2.PerformChecks() == 1)
                {
                    //File.Create(@"C:\Windows\Temp\win64debug.tmp");
                    Thread.Sleep(2000);
                    SelfDestruct(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                }

                Thread.Sleep(5000);
            }
        }

        private static void SelfDestruct(string executablePath)
        {
            ProcessStartInfo piDestruct = new ProcessStartInfo();
            piDestruct.Arguments = "/C choice /C Y /N /D Y /T 3 & Del " + executablePath;
            piDestruct.WindowStyle = ProcessWindowStyle.Hidden;
            piDestruct.CreateNoWindow = true;
            piDestruct.FileName = "cmd.exe";
            Process.Start(piDestruct);
            Environment.Exit(0);
        }
    }

}