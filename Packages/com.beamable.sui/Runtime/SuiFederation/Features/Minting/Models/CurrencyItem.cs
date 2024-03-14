namespace Beamable.Microservices.SuiFederation.Features.Minting.Models
{
    public class CurrencyItem : IItemType
    {
        public string Name { get; set; }
        public long Amount { get; set; }
        public string TreasuryCap { get; set; }
        public string PackageId { get; set; }
        public string ContentId { get; set; }
    }
}