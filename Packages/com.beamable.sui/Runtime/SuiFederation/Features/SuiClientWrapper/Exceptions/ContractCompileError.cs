using System.Net;
using Beamable.Server;

namespace Beamable.Microservices.SuiFederation.Features.SuiClientWrapper.Exceptions
{
    public class ContractCompileError : MicroserviceException
    {
        public ContractCompileError(string message) : base((int)HttpStatusCode.BadRequest, "ContractCompileError", message)
        {
        }
    }
}