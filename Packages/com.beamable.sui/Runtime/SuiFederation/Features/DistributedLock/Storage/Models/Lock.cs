using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.Microservices.SuiFederation.Features.DistributedLock.Storage.Models
{
    public record Lock
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime Expiry { get; set; }
    }
}