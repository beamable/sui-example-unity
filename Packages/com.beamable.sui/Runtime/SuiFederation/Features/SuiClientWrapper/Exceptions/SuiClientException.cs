using System.Net;
using Beamable.Server;

namespace Beamable.Microservices.SuiFederation.Features.SuiClientWrapper.Exceptions
{
    public class SuiClientException : MicroserviceException
    {
        public SuiClientException(string message) : base((int)HttpStatusCode.BadRequest, "SuiClientError", message)
        {
        }
    }
}