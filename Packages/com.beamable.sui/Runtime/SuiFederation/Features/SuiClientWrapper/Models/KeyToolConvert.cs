using Newtonsoft.Json;

namespace Beamable.Microservices.SuiFederation.Features.SuiClientWrapper.Models
{
    public class KeyToolConvert
    {
        [JsonProperty("bech32WithFlag")]
        public string Bench32Format { get; set; } = null!;

        [JsonProperty("base64WithFlag")]
        public string Base64Format { get; set; } = null!;

        [JsonProperty("hexWithoutFlag")]
        public string HexFormat { get; set; } = null!;

        [JsonProperty("scheme")]
        public string Scheme { get; set; } = null!;
    }
}