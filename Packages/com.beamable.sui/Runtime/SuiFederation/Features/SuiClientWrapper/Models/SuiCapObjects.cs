using System.Collections.Generic;

namespace Beamable.Microservices.SuiFederation.Features.SuiClientWrapper.Models
{
    public class SuiCapObjects
    {
        public List<SuiCapObject> GameAdminCaps { get; set; } = new();
        public List<SuiCapObject> TreasuryCaps { get; set; } = new();
    }

    public class SuiCapObject
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}