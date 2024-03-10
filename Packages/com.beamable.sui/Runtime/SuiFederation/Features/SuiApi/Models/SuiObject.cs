using System.Collections.Generic;
using Beamable.Microservices.SuiFederation.Features.Minting.Models;
using UnityEngine;

namespace Beamable.Microservices.SuiFederation.Features.SuiApi.Models
{
    public class SuiObject
    {
        [SerializeField]
        public string objectId;
        [SerializeField]
        public string type;
        [SerializeField]
        public string name;
        [SerializeField]
        public string description;
        [SerializeField]
        public string image_url;
        [SerializeField]
        public List<Attribute> attributes;
    }
}