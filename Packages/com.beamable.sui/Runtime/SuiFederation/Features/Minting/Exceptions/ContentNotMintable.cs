using System.Net;
using Beamable.Server;

namespace Beamable.Microservices.SuiFederation.Features.Minting.Exceptions
{
    internal class ContentNotMintable : MicroserviceException
    {
        public ContentNotMintable(string message) : base((int)HttpStatusCode.BadRequest, "ContentNotMintable",
            message)
        {
        }
    }
}