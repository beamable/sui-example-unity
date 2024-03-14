using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.Microservices.SuiFederation.Features.Minting.Storage.Models
{
    public record Mint
    {
        [BsonElement("_id")]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();

        public string PackageId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public DateTime Time { get; set; }
    }
}