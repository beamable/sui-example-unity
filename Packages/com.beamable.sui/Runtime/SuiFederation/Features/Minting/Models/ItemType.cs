namespace Beamable.Microservices.SuiFederation.Features.Minting.Models
{
    public interface IItemType
    {
        public string Name { get; set; }
        public string ContentId { get; set; }
        public string PackageId { get; set; }
    }
}