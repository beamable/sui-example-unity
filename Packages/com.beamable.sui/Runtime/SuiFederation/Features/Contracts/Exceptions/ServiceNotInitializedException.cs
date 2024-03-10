using System.Net;
using Beamable.Server;

namespace Beamable.Microservices.SuiFederation.Features.Contracts.Exceptions
{
    public class ServiceNotInitializedException : MicroserviceException
    {
        public ServiceNotInitializedException() : base((int)HttpStatusCode.BadRequest, "ServiceNotInitializedException", "Service is not yet initialized")
        {
        }
    }
}