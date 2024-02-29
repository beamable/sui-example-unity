using System.Collections.Generic;

namespace Beamable.Microservices.SuiFederation.Features.Minting
{
    public class MintRequestData
    {
        public string ContentId { get; set; }
        public uint Amount { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}