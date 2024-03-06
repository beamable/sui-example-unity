using System;
using System.Diagnostics;
using Beamable.Common;
using Beamable.Microservices.SuiFederation.Features.ExecWrapper.Exceptions;

namespace Beamable.Microservices.SuiFederation.Features.ExecWrapper
{
    public static class ExecCommand
    {
        private const int ProcessTimeoutMs = 10000;

        public static void RunSdkCompilation()
        {
            try
            {
                var workingDirectory = "/subapp/sui_ts";
                BeamableLogger.Log("Running SdkCompilation process...");
                ExecuteShell("apk add -q icu-data-full",workingDirectory);
                BeamableLogger.Log("Installing NodeJS.");
                ExecuteShell("apk add -q nodejs npm",workingDirectory);
                ExecuteShell("apk add -q nodejs-current",workingDirectory);
                BeamableLogger.Log("Installing NodeJS modules.");
                ExecuteShell("npm install -q",workingDirectory);
                BeamableLogger.Log("Running NodeJS build.");
                ExecuteShell("npm run build",workingDirectory);
                BeamableLogger.Log("Done running SdkCompilation process.");
            }
            catch (Exception e)
            {
                BeamableLogger.LogError("RunSdkCompilation error: {processOutput}", e.Message);
                throw new ExecCommandException(e.Message);
            }
        }

        public static void RunSuiClientCompilation()
        {
            try
            {
                var workingDirectory = "/subapp/move";
                BeamableLogger.Log("Running SuiClientCompilation process...");
                ExecuteShell("cp libs/sgerrand.rsa.pub /etc/apk/keys/sgerrand.rsa.pub",workingDirectory);
                ExecuteShell("apk add -q libs/glibc-2.35-r1.apk libs/glibc-bin-2.35-r1.apk libs/glibc-i18n-2.35-r1.apk",workingDirectory);
                ExecuteShell("/usr/glibc-compat/bin/localedef -i en_US -f UTF-8 en_US.UTF-8",workingDirectory);
                ExecuteShell("apk add -q gcompat",workingDirectory);
                ExecuteShell("apk add -q git",workingDirectory);
                ExecuteShell("rm /usr/lib/libstdc++.so.6 /usr/lib/libstdc++.so.6.0.30",workingDirectory);
                ExecuteShell("cp libs/libstdc++.so.6.0.30 /usr/lib/libstdc++.so.6.0.30",workingDirectory);
                ExecuteShell("ln -s /usr/lib/libstdc++.so.6.0.30 /usr/lib/libstdc++.so.6",workingDirectory);
                BeamableLogger.Log("Done running SuiClientCompilation process.");
            }
            catch (Exception e)
            {
                BeamableLogger.LogError("SuiClientCompilation error: {processOutput}", e.Message);
                throw new ExecCommandException(e.Message);
            }
        }

        private static void ExecuteShell(string command, string workingDirectory = null)
        {
            Execute("/bin/sh", $"-c \"{command}\"", workingDirectory);
        }

        private static void Execute(string program, string args, string workingDirectory = null)
        {
            using var process = new Process();
            process.StartInfo =
                new ProcessStartInfo(program, args)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

            if (workingDirectory is not null)
                process.StartInfo.WorkingDirectory = workingDirectory;

            process.Start();
            process.WaitForExit(ProcessTimeoutMs);
        }
    }
}