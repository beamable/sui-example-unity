using System.Collections.Generic;

namespace Beamable.Microservices.SuiFederation.Features.Minting.Models
{
    public class InventoryMintRequest
    {
        public List<CurrencyItem> CurrencyItems { get; set; } = new();
        public List<GameItem> GameItems { get; set; } = new();
    }
}