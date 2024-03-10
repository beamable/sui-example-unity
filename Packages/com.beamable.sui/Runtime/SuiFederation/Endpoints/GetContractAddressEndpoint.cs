using System.Threading.Tasks;
using Beamable.Microservices.SuiFederation.Features.Contracts;

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
            return contract?.PackageId;
        }
    }
}