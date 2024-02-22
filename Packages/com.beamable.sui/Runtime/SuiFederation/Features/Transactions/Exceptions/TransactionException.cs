using System.Net;
using Beamable.Server;

namespace Beamable.Microservices.SuiFederation.Features.Transactions.Exceptions
{
    internal class TransactionException : MicroserviceException
    {
        public TransactionException(string message) : base((int)HttpStatusCode.BadRequest, "TransactionError", message)
        {
        }
    }
}