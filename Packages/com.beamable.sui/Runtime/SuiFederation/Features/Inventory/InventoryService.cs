using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Microservices.SuiFederation.Features.Contracts;
using Beamable.Microservices.SuiFederation.Features.Inventory.Models;
using Beamable.Microservices.SuiFederation.Features.Minting;
using Beamable.Microservices.SuiFederation.Features.SuiApi;
namespace Beamable.Microservices.SuiFederation.Features.Inventory
{
    public class InventoryService : IService
    {
        private readonly SuiApiService _suiApiService;
        private readonly ContractProxy _contractProxy;

        public InventoryService(SuiApiService suiApiService, ContractProxy contractProxy)
        {
            _suiApiService = suiApiService;
            _contractProxy = contractProxy;
        }

        public async Task<FederatedInventoryProxyState> GetInventoryState(string id)
        {
            var contracts = await _contractProxy.GetContracts();

            var currencyRequest = new CurrencyRequest();
            foreach (var contract in contracts)
            {
                if (contract.TreasuryCaps is not null)
                {
                    currencyRequest.CurrencyModules.Add(new CurrencyModule
                    {
                        PackageId = contract.PackageId,
                        ModuleNames = contract.TreasuryCaps.Select(t => t.Name).ToList()
                    });
                }
            }

            var coinBalance = await _suiApiService.GetBalance(id, currencyRequest);
            var suiObjects = await _suiApiService.GetOwnedObjects(id, contracts.Select(c => c.PackageId).ToArray());

            var items = new List<(string, FederatedItemProxy)>();
            var currencies = coinBalance.coins?.ToDictionary(coin => GetCurrencyContenId(coin.coinType), coin => coin.total);

            foreach (var suiObject in suiObjects)
            {
                items.Add((GetItemContenId(suiObject.type),
                        new FederatedItemProxy
                        {
                            proxyId = suiObject.objectId,
                            properties = suiObject.GetProperties().ToList()
                        }
                    ));
            }

            var itemGroups = items
                .GroupBy(i => i.Item1)
                .ToDictionary(g => g.Key, g => g.Select(i => i.Item2).ToList());

            return new FederatedInventoryProxyState
            {
                currencies = currencies,
                items = itemGroups
            };
        }

        private string GetItemContenId(string itemName)
        {
            return $"items.blockchain_currency.{itemName}";
        }

        private string GetCurrencyContenId(string currencyName)
        {
            return $"currency.blockchain_currency.{currencyName}";
        }
    }
}