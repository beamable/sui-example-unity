using System.Collections.Generic;

namespace Beamable.Microservices.SuiFederation.Features.Inventory.Models
{
    public class CurrencyRequest
    {
        public List<CurrencyModule> CurrencyModules { get; set; } = new();
    }

    public class CurrencyModule
    {
        public List<string> ModuleNames { get; set; } = new();
        public string PackageId { get; set; }
    }
}