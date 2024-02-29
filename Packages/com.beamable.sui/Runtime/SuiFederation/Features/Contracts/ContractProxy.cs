using System.Threading.Tasks;
using Beamable.Microservices.SuiFederation.Features.Contracts.Exceptions;
using Beamable.Microservices.SuiFederation.Features.Contracts.Storage;
using Beamable.Microservices.SuiFederation.Features.Contracts.Storage.Models;

namespace Beamable.Microservices.SuiFederation.Features.Contracts
{
    public class ContractProxy : IService
    {
        private readonly ContractCollection _contractCollection;
        private Contract? _cachedDefaultContract;

        public ContractProxy(ContractCollection contractCollection)
        {
            _contractCollection = contractCollection;
        }

        public async ValueTask<Contract> GetDefaultContractOrDefault()
        {
            if (_cachedDefaultContract is null)
            {
                var persistedContract = await _contractCollection.GetContract(ContractService.DefaultContractName);
                _cachedDefaultContract = persistedContract;
            }

            return _cachedDefaultContract;
        }

        public async ValueTask<Contract> GetDefaultContract()
        {
            var contract = await GetDefaultContractOrDefault();
            if (contract is null)
                throw new ContractNotInitializedException();
            return contract;
        }

        public async Task InitializeDefaultContract(string publicKey)
        {
            _cachedDefaultContract = null;
            await _contractCollection.SaveContract(new Contract
            {
                Name = ContractService.DefaultContractName,
                PublicKey = publicKey
            });
        }
    }
}