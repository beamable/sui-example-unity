using System.Net;
using Beamable.Server;

namespace Beamable.Microservices.SuiFederation.Features.ExecWrapper.Exceptions
{
    public class ExecCommandException : MicroserviceException
    {
        public ExecCommandException(string message) : base((int)HttpStatusCode.BadRequest, "ExecCommandException", message)
        {
        }
    }
}