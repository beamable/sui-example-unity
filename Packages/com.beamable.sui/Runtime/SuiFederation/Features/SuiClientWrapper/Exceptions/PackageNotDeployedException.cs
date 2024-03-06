using System.Net;
using Beamable.Server;

namespace Beamable.Microservices.SuiFederation.Features.SuiClientWrapper.Exceptions
{
    public class PackageNotDeployedException : MicroserviceException
    {
        public PackageNotDeployedException(string message) : base((int)HttpStatusCode.BadRequest, "SuiClientError", message)
        {
        }
    }
}