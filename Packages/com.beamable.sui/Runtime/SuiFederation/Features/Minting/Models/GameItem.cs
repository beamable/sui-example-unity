namespace Beamable.Microservices.SuiFederation.Features.Minting.Models
{
    public class GameItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public string GameAdminCap { get; set; }
        public string PackageId { get; set; }
        public string ContentName { get; set; }
        public Attribute[] Attributes { get; set; }
    }

    public class Attribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}