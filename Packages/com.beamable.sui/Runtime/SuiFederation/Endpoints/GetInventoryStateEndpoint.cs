using Beamable.Common;
using Beamable.Microservices.SuiFederation.Features.Inventory;

namespace Beamable.Microservices.SuiFederation.Endpoints
{
    public class GetInventoryStateEndpoint : IEndpoint
    {
        private readonly InventoryService _inventoryService;

        public GetInventoryStateEndpoint(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Promise<FederatedInventoryProxyState> GetInventoryState(string id)
        {
            return await _inventoryService.GetInventoryState(id);
        }
    }
}