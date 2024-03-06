using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.Microservices.SuiFederation.Features.Contracts.Storage.Models
{
    public record Contract
    {
        [BsonElement("_id")]
        public string Name { get; set; }
        public string PackageId { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public List<CapObject> GameAdminCaps { get; set; }

        public List<CapObject> TreasuryCaps { get; set; }
    }

    public record CapObject
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}