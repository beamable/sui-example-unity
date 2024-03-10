namespace Beamable.Microservices.SuiFederation.Features.Contracts.Models
{
    public class CurrencyModuleData : IModuleData
    {
        public string module_name { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string description { get; set; }
    }
}