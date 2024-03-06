using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Microservices.SuiFederation.Features.Accounts;
using Beamable.Microservices.SuiFederation.Features.Accounts.Models;
using Beamable.Microservices.SuiFederation.Features.Contracts.Exceptions;
using Beamable.Microservices.SuiFederation.Features.Contracts.Storage.Models;
using Beamable.Microservices.SuiFederation.Features.ExecWrapper;
using Beamable.Microservices.SuiFederation.Features.SuiClientWrapper;

namespace Beamable.Microservices.SuiFederation.Features.Contracts
{
    public class ContractService : IService
    {
        public const string DefaultContractName = "default";
        private readonly ContractProxy _contractProxy;
        private readonly AccountsService _accountsService;

        public ContractService(ContractProxy contractProxy, AccountsService accountsService)
        {
            _contractProxy = contractProxy;
            _accountsService = accountsService;
        }

        public async ValueTask<Contract> GetOrCreateDefaultContract()
        {
            try
            {
                return await GetOrCreateContract();
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError($"{DefaultContractName} create exception {ex.Message}");
                throw new ContractCreateException();
            }
        }

        private async ValueTask<Contract> GetOrCreateContract()
        {
            var persistedContract = await _contractProxy.GetDefaultContractOrDefault();
            if (persistedContract is not null)
            {
                return persistedContract;
            }

            //Install SUI Client compatibility layer
            ExecCommand.RunSuiClientCompilation();

            var realmAccount = await _accountsService.GetOrCreateRealmAccount();
            var contractDeployOutput = await SuiClient.Compile(realmAccount);
            var contractCaps = contractDeployOutput.GetCapObjects();

            var contract = new Contract
            {
                Name = DefaultContractName,
                PackageId = contractDeployOutput.GetPackageId(),
                GameAdminCaps = contractCaps.GameAdminCaps.Select(c => new CapObject
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList(),
                TreasuryCaps = contractCaps.TreasuryCaps.Select(c => new CapObject
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };

            await _contractProxy.InitializeDefaultContract(contract);
            return contract;
        }




    }
}