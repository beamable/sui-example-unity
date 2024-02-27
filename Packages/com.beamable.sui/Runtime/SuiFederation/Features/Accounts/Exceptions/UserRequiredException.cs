using System.Net;
using Beamable.Server;

namespace Beamable.Microservices.SuiFederation.Features.Accounts.Exceptions
{
    public class UserRequiredException : MicroserviceException

    {
        public UserRequiredException() : base((int)HttpStatusCode.Unauthorized, "UserRequired", "")
        {
        }
    }
}