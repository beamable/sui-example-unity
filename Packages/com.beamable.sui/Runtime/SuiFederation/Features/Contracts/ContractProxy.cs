using System.Linq;
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

        public async Task InitializeDefaultContract(Contract contract)
        {
            _cachedDefaultContract = null;
            await _contractCollection.TryInsertContract(contract);
        }

        public async ValueTask<string> GetGameCap(string name)
        {
            var contract = await GetDefaultContract();
            return contract.GameAdminCaps.SingleOrDefault(x => x.Name == name)?.Id;
        }

        public async ValueTask<string> GetTreasuryCap(string name)
        {
            var contract = await GetDefaultContract();
            return contract.TreasuryCaps.SingleOrDefault(x => x.Name == name)?.Id;
        }
    }
}