using System;
using System.Collections.Generic;
using System.Linq;
using Beamable.Microservices.SuiFederation.Features.Minting.Storage.Models;

namespace Beamable.Microservices.SuiFederation.Features.Minting.Models
{
    public class InventoryMintRequest
    {
        public List<CurrencyItem> CurrencyItems { get; set; } = new();
        public List<GameItem> GameItems { get; set; } = new();

        public IEnumerable<Mint> ToMints(string owner)
        {
            return CurrencyItems
                .Concat(GameItems.Cast<IItemType>())
                .Select(item => new Mint
                {
                    Name = item.ContentId,
                    PackageId = item.PackageId,
                    Owner = owner,
                    Time = DateTime.UtcNow
                });
        }
    }
}