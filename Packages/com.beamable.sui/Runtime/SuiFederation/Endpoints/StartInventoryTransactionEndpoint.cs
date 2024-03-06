using System.Collections.Generic;
using System.Linq;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.Microservices.SuiFederation.Features.Inventory;
using Beamable.Microservices.SuiFederation.Features.Minting;
using Beamable.Microservices.SuiFederation.Features.Transactions;

namespace Beamable.Microservices.SuiFederation.Endpoints
{
    public class StartInventoryTransactionEndpoint : IService
    {
        private readonly TransactionManager _transactionManager;
        private readonly InventoryService _inventoryService;
        private readonly MintingService _mintingService;

        public StartInventoryTransactionEndpoint(TransactionManager transactionManager, InventoryService inventoryService, MintingService mintingService)
        {
            _transactionManager = transactionManager;
            _inventoryService = inventoryService;
            _mintingService = mintingService;
        }

        public async Promise<FederatedInventoryProxyState> StartInventoryTransaction(string id, long userId, string transaction,
            Dictionary<string, long> currencies, List<FederatedItemCreateRequest> newItems,
            List<FederatedItemDeleteRequest> deleteItems, List<FederatedItemUpdateRequest> updateItems)
        {
            return await _transactionManager.WithTransactionAsync(id, transaction, userId, async () =>
            {
                if (currencies.Any() || newItems.Any())
                {
                    var currencyMints = currencies
                        .Select(c => new MintRequestData
                        {
                            ContentId = c.Key,
                            Amount = (uint)c.Value,
                            Properties = new Dictionary<string, string>()
                        });

                    var itemMints = newItems.Select(i => new MintRequestData
                    {
                        ContentId = i.contentId,
                        Amount = 1,
                        Properties = i.properties
                    });

                    await _mintingService.Mint(userId, id, transaction,
                        currencyMints.Union(itemMints).ToList());
                }

                return await _inventoryService.GetInventoryState(id);
            });
        }
    }
}