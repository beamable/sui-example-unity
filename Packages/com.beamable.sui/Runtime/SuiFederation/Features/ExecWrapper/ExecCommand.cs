using System;
using System.Diagnostics;
using System.Threading.Tasks;
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