using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Microservices.SuiFederation.Features.Accounts.Models;
using Beamable.Microservices.SuiFederation.Features.SuiClientWrapper.Exceptions;
using Beamable.Microservices.SuiFederation.Features.SuiClientWrapper.Models;
using Newtonsoft.Json;

namespace Beamable.Microservices.SuiFederation.Features.SuiClientWrapper
{
    public static class SuiClient
    {
        private static bool _initialized;
        private const int ProcessTimeoutMs = 5000;
        private const int FaucetWaitTimeSec = 20;
        private const string WorkingDirectory = "/subapp/move";
        private const string ExecutableAmd64 = "sui-x64-1.18.1";
        private const string ExecutableArm64 = "sui-arm64-1.18.1";

        public static async Task<MoveDeployOutput> Compile(Account realmAccount)
        {
            await Initialize(realmAccount);

            BeamableLogger.Log("Compiling smart contract...");
            await Execute(GetExecutable(), $"move build", ignoreOutput: true);
            BeamableLogger.Log("Deploying smart contract...");
            var deployOutputData = await Execute(GetExecutable(), $"client --client.config client.yaml publish --silence-warnings --json --gas-budget 50000000 sources/default.move", ignoreOutputError: true);
            var deployOutput = JsonConvert.DeserializeObject<MoveDeployOutput>(deployOutputData);
            BeamableLogger.Log($"Deploy output package {deployOutput.GetPackageId()}");
            return deployOutput;
        }

        private static async ValueTask Initialize(Account realmAccount)
        {
            if (!_initialized)
            {
                BeamableLogger.Log("Extracting Sui CLI executables...");
                await ExecuteShell("tar -xvzf sui-x64.tar.gz", WorkingDirectory);
                await ExecuteShell("tar -xvzf sui-arm64.tar.gz", WorkingDirectory);
                BeamableLogger.Log("Changing permissions of Move/sui");
                await ExecuteShell($"chmod -R 755 {GetExecutable()}");
                BeamableLogger.Log("Importing account...");
                await Execute(GetExecutable(), $"client --client.config client.yaml switch --env {Configuration.SuiEnvironment}");
                var convertedKey = await Execute(GetExecutable(), $"keytool --keystore-path sui.keystore convert --json {realmAccount.PrivateKey}");
                var keyToolConvert = JsonConvert.DeserializeObject<KeyToolConvert>(convertedKey);
                BeamableLogger.Log("Configuring SUI client tool...");
                await Execute(GetExecutable(), $"keytool --keystore-path sui.keystore import {keyToolConvert.Bench32Format} ed25519");
                await Execute(GetExecutable(), $"client --client.config client.yaml switch --address {realmAccount.Address}");

                if (Configuration.SuiEnvironment == "devnet")
                {
                    BeamableLogger.Log($"Requesting faucet coins, waiting {FaucetWaitTimeSec} sec...");
                    await Execute(GetExecutable(), $"client --client.config client.yaml faucet");
                    await Task.Delay(TimeSpan.FromSeconds(FaucetWaitTimeSec));
                    BeamableLogger.Log("Done requesting faucet coins.");
                }
                _initialized = true;
            }
        }

        private static async Task<string> ExecuteShell(string command, string workingDirectory = null)
        {
            return await Execute("/bin/sh", $"-c \"{command}\"", workingDirectory);
        }

        private static async Task<string> Execute(string program, string args, string workingDirectory = WorkingDirectory, bool ignoreOutput = false, bool ignoreOutputError = false)
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

            if (!ignoreOutput)
            {

                var outputText = await process.StandardOutput.ReadToEndAsync();
                var outputError = await process.StandardError.ReadToEndAsync();

                process.WaitForExit(ProcessTimeoutMs);

                if (!string.IsNullOrEmpty(outputError) && !ignoreOutputError)
                {
                    BeamableLogger.LogError("Process error: {processOutput}", outputError);
                    throw new SuiClientException(outputError);
                }

                BeamableLogger.Log("Process output: {processOutput}", outputText);
                return outputText;
            }
            process.WaitForExit(ProcessTimeoutMs);
            return string.Empty;
        }

        private static string GetExecutable()
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => Path.Combine(WorkingDirectory, ExecutableAmd64),
                Architecture.Arm64 => Path.Combine(WorkingDirectory, ExecutableArm64),
                _ => throw new SuiClientException($"{RuntimeInformation.ProcessArchitecture} is not supported")
            };
        }
    }
}