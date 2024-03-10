using System.Threading.Tasks;
using Beamable.Microservices.SuiFederation.Features.Contracts;
using Beamable.Microservices.SuiFederation.Features.Contracts.Exceptions;

namespace Beamable.Microservices.SuiFederation.Endpoints
{
    public class GetContractAddressEndpoint : IEndpoint
    {
        private readonly ContractProxy _contractProxy;

        public GetContractAddressEndpoint(ContractProxy contractProxy)
        {
            _contractProxy = contractProxy;
        }

        public async Task<string> GetContractAddress(string name)
        {
            var contract = await _contractProxy.GetContract(name);
            if (contract is not null)
            {
                return contract.PackageId;
            }
            throw new ContractNotFoundException(name);
        }
    }
}