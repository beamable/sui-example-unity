using System.Collections.Generic;

namespace Beamable.Microservices.SuiFederation.Features.Contracts.Models
{
    public class ItemModuleData : IModuleData
    {
        public string module_name { get; set; }
        public List<string> customFields { get; set; } = new();
    }
}