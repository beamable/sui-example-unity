﻿using System.Net;
using Beamable.Server;

namespace Beamable.Microservices.SuiFederation.Features.EthRpc.Exceptions
{
    internal class ContractException : MicroserviceException
    {
        public ContractException(string message) : base((int)HttpStatusCode.InternalServerError, "ContractError",
            message)
        {
        }
    }
}