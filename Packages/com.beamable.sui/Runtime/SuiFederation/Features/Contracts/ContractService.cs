using System;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Microservices.SuiFederation.Features.Accounts;
using Beamable.Microservices.SuiFederation.Features.Contracts.Exceptions;
using Beamable.Microservices.SuiFederation.Features.Contracts.Models;
using Beamable.Microservices.SuiFederation.Features.Contracts.Storage.Models;
using Beamable.Microservices.SuiFederation.Features.DistributedLock;
using Beamable.Microservices.SuiFederation.Features.ExecWrapper;
using Beamable.Microservices.SuiFederation.Features.SuiClientWrapper;
using Beamable.Sui.Common.Content;
using Beamable.Server.Api.Content;

namespace Beamable.Microservices.SuiFederation.Features.Contracts
{
    public class ContractService : IService
    {
        private readonly ContractProxy _contractProxy;
        private readonly AccountsService _accountsService;
        private readonly ContractTemplateService _contractTemplateService;
        private readonly SuiClient _suiClient;
        private readonly IMicroserviceContentApi _contentApi;
        private readonly LockManagerFactory _lockManagerFactory;
        private bool _initialized;

        public ContractService(ContractProxy contractProxy, AccountsService accountsService, ContractTemplateService contractTemplateService, SuiClient suiClient, IMicroserviceContentApi contentApi, LockManagerFactory lockManagerFactory)
        {
            _contractProxy = contractProxy;
            _accountsService = accountsService;
            _contractTemplateService = contractTemplateService;
            _suiClient = suiClient;
            _contentApi = contentApi;
            _lockManagerFactory = lockManagerFactory;
        }

        public async ValueTask<Contract> GetOrCreateModuleContract(IModuleData data)
        {
            try
            {
                return await GetOrCreateContract(data);
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError($"{data.module_name} create exception {ex.Message}");
                throw new ContractCreateException();
            }
        }

        public async Task InitializeContentContracts()
        {
            var manifest = await _contentApi.GetManifest();
            foreach (var clientContentInfo in manifest.entries)
            {
                if (clientContentInfo.contentId.StartsWith("items.blockchain_item") ||
                    clientContentInfo.contentId.StartsWith("currency.blockchain_currency"))
                {
                    var contentObject = await _contentApi.GetContent(clientContentInfo.contentId);
                    if (contentObject is BlockchainItem blockchainItem)
                    {
                        var contractData = new ItemModuleData
                        {
                            contract_name = clientContentInfo.contentId,
                            module_name =  contentObject.Id.GetContentIdName()
                        };
                        await GetOrCreateContract(contractData);
                    }
                    if (contentObject is BlockchainCurrency blockchainCurrency)
                    {
                        var currency = contentObject as BlockchainCurrency;
                        var contractData = new CurrencyModuleData
                        {
                            contract_name = clientContentInfo.contentId,
                            module_name =  contentObject.Id.GetContentIdName(),
                            name = currency.Name,
                            symbol = currency.Symbol,
                            description = currency.Description
                        };
                        await GetOrCreateContract(contractData);
                    }
                }
            }
        }

        private async ValueTask<Contract> GetOrCreateContract(IModuleData data)
        {
            var persistedContract = await _contractProxy.GetContract(data.contract_name);
            if (persistedContract is not null)
            {
                return persistedContract;
            }

            var lockManager = _lockManagerFactory.Create($"{data.contract_name}");
            try
            {
                if (await lockManager.AcquireLock(300))
                {
                    if (!_initialized)
                    {
                        //Install SUI Client compatibility layer
                        await ExecCommand.RunSuiClientCompilation();
                        _initialized = true;
                    }

                    await ExecCommand.RunCopySuiClientCompilation();

                    var realmAccount = await _accountsService.GetOrCreateRealmAccount();

                    switch (data)
                    {
                        case ItemModuleData itemModuleData:
                            await _contractTemplateService.GenerateItemContract(itemModuleData);
                            break;
                        case CurrencyModuleData currencyModuleData:
                            await _contractTemplateService.GenerateCurrencyContract(currencyModuleData);
                            break;
                    }

                    BeamableLogger.Log($"Creating contract for {data.module_name}");
                    var contractDeployOutput = await _suiClient.Compile(data.module_name, realmAccount);
                    var contractCaps = contractDeployOutput.GetCapObjects();

                    var contract = new Contract
                    {
                        Name = data.contract_name,
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

                    await _contractProxy.InitializeContract(contract);
                    BeamableLogger.Log($"Done creating contract for {data.module_name}");
                    return contract;
                }
            }
            finally
            {
                await ExecCommand.RunCleanupSuiClientCompilation();
                await lockManager.ReleaseLock();
            }
            return null;
        }




    }
}