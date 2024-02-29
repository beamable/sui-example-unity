using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Microservices.SuiFederation.Features.Accounts;
using Beamable.Microservices.SuiFederation.Features.Contracts.Exceptions;
using Beamable.Microservices.SuiFederation.Features.Contracts.Storage.Models;

namespace Beamable.Microservices.SuiFederation.Features.Contracts
{
    public class ContractService : IService
    {
        private const string DefaultErc1155Path = "Move/Contracts/default.move";
        private static readonly string DefaultContractSource = File.ReadAllText(DefaultErc1155Path);
        public const string DefaultContractName = "default";
        private readonly ContractProxy _contractProxy;
        private readonly AccountsService _accountsService;

        private static readonly SemaphoreSlim Semaphore = new(1);

        public ContractService(ContractProxy contractProxy, AccountsService accountsService)
        {
            _contractProxy = contractProxy;
            _accountsService = accountsService;
        }

        public async ValueTask<Contract> GetOrCreateDefaultContract()
        {
            await Semaphore.WaitAsync();
            try
            {
                return await GetOrCreateContract(DefaultContractName, DefaultContractSource);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        private async Task<Contract> GetOrCreateContract(string name, string contractFile)
        {
            var persistedContract = await _contractProxy.GetDefaultContractOrDefault();
            if (persistedContract is not null)
            {
                return persistedContract;
            }

            var realmAccount = await _accountsService.GetOrCreateRealmAccount();

            // var compilerOutput = await Compile(contractFile);
            // var contractOutput = compilerOutput.Contracts.Contract.First().Value;
            // var abi = contractOutput.GetAbi();
            // var contractByteCode = contractOutput.GetBytecode();

            //var gas = await _ethRpcClient.EstimateContractGasAsync(realmAccount, abi, contractByteCode);
            //var result = await _ethRpcClient.DeployContractAsync(realmAccount, abi, contractByteCode, gas);

            var contract = new Contract
            {
                Name = name,
                PublicKey = ""//result.ContractAddress
            };

            await _contractProxy.InitializeDefaultContract(contract.PublicKey);
            persistedContract = await _contractProxy.GetDefaultContractOrDefault();

            if (persistedContract is not null)
            {
                BeamableLogger.Log("Contract {contractName} created successfully. Address: {contractAddress}", name, contract.PublicKey);
                return contract;
            }

            BeamableLogger.LogWarning("Contract {contractName} already created, fetching again", name);
            return await GetOrCreateContract(name, contractFile);
        }


    }
}