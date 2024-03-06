using System.Collections.Generic;
using System.Linq;
using Beamable.Microservices.SuiFederation.Features.SuiClientWrapper.Exceptions;
using Newtonsoft.Json;

namespace Beamable.Microservices.SuiFederation.Features.SuiClientWrapper.Models
{
    public class MoveDeployOutput
    {
        [JsonProperty("objectChanges")]
        public List<CreatedObjectsOutput> CreatedObjects { get; set; } = new ();

        public string GetPackageId()
        {
            var package = CreatedObjects.FirstOrDefault(obj => obj.PackageId != null);
            if (package is not null)
            {
                return package.PackageId;
            }
            throw new PackageNotDeployedException("PackageId not found.");
        }

        public SuiCapObjects GetCapObjects()
        {
            var capObject = new SuiCapObjects();
            var packageId = GetPackageId();
            foreach (var obj in CreatedObjects)
            {
                if (obj.ObjectType is null)
                    continue;
                if (obj.ObjectType.StartsWith(packageId) && obj.ObjectType.EndsWith("GameAdminCap"))
                {
                    var parts = obj.ObjectType.Split("::");
                    capObject.GameAdminCaps.Add(new SuiCapObject
                    {
                        Id = obj.ObjectId,
                        Name = parts[1]
                    });
                }
                if (obj.ObjectType.StartsWith($"0x2::coin::TreasuryCap<{packageId}"))
                {
                    var parts = obj.ObjectType.Split("::");
                    capObject.TreasuryCaps.Add(new SuiCapObject
                    {
                        Id = obj.ObjectId,
                        Name = parts[3]
                    });
                }
            }
            return capObject;
        }
    }

    public class CreatedObjectsOutput
    {
        [JsonProperty("digest")]
        public string Digest { get; set; } = null!;

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("objectType")]
        public string ObjectType { get; set; }

        [JsonProperty("packageId")]
        public string PackageId { get; set; }
    }
}