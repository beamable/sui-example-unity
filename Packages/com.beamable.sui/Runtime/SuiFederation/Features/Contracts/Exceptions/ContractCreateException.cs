using System.Net;
using Beamable.Server;

namespace Beamable.Microservices.SuiFederation.Features.Contracts.Exceptions
{
    internal class ContractCreateException : MicroserviceException
    {
        public ContractCreateException() : base((int)HttpStatusCode.BadRequest, "ContractCreateException", "Contract cannot be created")
        {
        }
    }
}