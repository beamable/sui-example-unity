using System.Net;
using Beamable.Server;

namespace Beamable.Microservices.SuiFederation.Features.Contracts.Exceptions
{
    public class ContractNotFoundException : MicroserviceException
    {
        public ContractNotFoundException(string name) : base((int)HttpStatusCode.BadRequest, "ContractNotFoundException", $"Contract for {name} not found.")
        {
        }
    }
}