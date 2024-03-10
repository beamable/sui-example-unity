using Newtonsoft.Json;

namespace Beamable.Microservices.SuiFederation.Features.SuiClientWrapper.Models
{
    public class GasBalanceItem
    {
        [JsonProperty("gasCoinId")]
        public string GasCoinId { get; set; }

        [JsonProperty("gasBalance")]
        public long GasBalance { get; set; }
    }
}