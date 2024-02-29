using System.Net;
using Beamable.Server;

namespace Beamable.Microservices.SuiFederation.Features.Minting
{
    public class UndefinedItemException : MicroserviceException
    {
        public UndefinedItemException(string message) : base((int)HttpStatusCode.BadRequest, "UndefinedItemException",
            message)
        {
        }
    }
}