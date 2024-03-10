using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Newtonsoft.Json;
using UnityEngine;

namespace Beamable.Sui.Common.Content
{
    /// <summary>
    /// BlockchainItem
    /// </summary>
    [ContentType("blockchain_item")]
    public class BlockchainItem : ItemContent
    {
        public BlockchainItem()
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
        [SerializeField] private string _url;
        [SerializeField][TextArea(10, 10)] private string _description;

        /// <summary>
        /// NFT name
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// NFT image
        /// </summary>
        public string Url => _url;

        /// <summary>
        /// NFT description
        /// </summary>
        public string Description => _description;

        /// <summary>
        /// Creates a JSON string that represents the NFT metadata
        /// </summary>
        /// <returns></returns>
        public string ToMetadataJsonString()
        {
            var metadata = new
            {
                Name,
                Description,
                Url
            };
            return JsonConvert.SerializeObject(metadata);
        }
    }
}