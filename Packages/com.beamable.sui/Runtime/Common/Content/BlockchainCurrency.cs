using Beamable.Common.Content;
using Beamable.Common.Inventory;
using UnityEngine;

namespace Beamable.Sui.Common.Content
{
    /// <summary>
    /// BlockchainCurrency
    /// </summary>
    [ContentType(ContentTypeConfiguration.CurrencyTypeName)]
    public class BlockchainCurrency : CurrencyContent
    {
        public BlockchainCurrency()
        {
            federation = new OptionalFederation
            {
                HasValue = true,
                Value = new Federation
                {
                    Service = "SuiFederation",
                    Namespace = "sui"
                }
            };
        }

        [SerializeField] private string _name;
        [SerializeField] private string _symbol;
        [SerializeField] private string _description;

        /// <summary>
        /// Currency name
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Currency symbol
        /// </summary>
        public string Symbol => _symbol;

        /// <summary>
        /// Currency description
        /// </summary>
        public string Description => _description;
    }
}