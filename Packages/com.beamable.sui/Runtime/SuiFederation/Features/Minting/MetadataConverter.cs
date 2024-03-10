using System.Collections.Generic;
using System.Linq;
using Beamable.Common.Api.Inventory;
using Beamable.Microservices.SuiFederation.Features.Minting.Models;
using Beamable.Microservices.SuiFederation.Features.SuiApi.Models;
using Beamable.Sui.Common.Content;

namespace Beamable.Microservices.SuiFederation.Features.Minting
{
    internal static class MetadataConverter
    {
        public static GameItem ToGameItem(this MintRequestData request, BlockchainItem? contentDefinition)
        {
            return new GameItem
            {
                Name = contentDefinition?.Name ?? request.ContentId,
                Description = contentDefinition?.Description ?? "",
                ImageURL = contentDefinition?.Url ?? "",
                ContentName = contentDefinition?.ContentName,
                // Attributes = contentDefinition?.CustomProperties
                //     .Select(kv => new Attribute
                //     {
                //         Name = kv.Key,
                //         Value = kv.Value
                //     }).ToArray()
            };
        }

        public static CurrencyItem ToCurrencyItem(this MintRequestData request, BlockchainCurrency? contentDefinition)
        {
            return new CurrencyItem()
            {
                Name = contentDefinition?.ContentName,
                Amount = request.Amount,
            };
        }

        public static IEnumerable<ItemProperty> GetProperties(this SuiObject suiObject)
        {
            var properties = new List<ItemProperty>();

            if (!string.IsNullOrEmpty(suiObject.name))
                properties.Add(new ItemProperty { name = "Name", value = suiObject.name });

            if (!string.IsNullOrEmpty(suiObject.description))
                properties.Add(new ItemProperty { name = "Description", value = suiObject.description });

            if (!string.IsNullOrEmpty(suiObject.image_url))
                properties.Add(new ItemProperty { name = "Url", value = suiObject.image_url });

            return properties;
        }
    }
}