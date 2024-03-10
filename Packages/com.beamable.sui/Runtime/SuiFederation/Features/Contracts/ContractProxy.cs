using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Microservices.SuiFederation.Features.Contracts.Models;
using Beamable.Microservices.SuiFederation.Features.Contracts.Storage;
using Beamable.Microservices.SuiFederation.Features.Contracts.Storage.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Beamable.Microservices.SuiFederation.Features.Contracts
{
    public class ContractProxy : IService
    {
        private readonly ContractCollection _contractCollection;
        private readonly MemoryCache _contractCache = new(Options.Create(new MemoryCacheOptions()));

        public ContractProxy(ContractCollection contractCollection)
        {
            _contractCollection = contractCollection;
        }

        public async Task<Contract?> GetContract(string contractName)
        {
            return await _contractCache.GetOrCreateAsync(contractName, async cacheEntry =>
            {
                var contract = await _contractCollection.GetContract(contractName);
                if (contract is null)
                {
                    cacheEntry.Dispose();
                    return null;
                }
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(5);
                return contract;
            });
        }

        public async Task<List<Contract>> GetContracts()
        {
            return await _contractCollection.GetContracts();
        }

        public async Task InitializeContract(Contract contract)
        {
            await _contractCollection.TryInsertContract(contract);
        }

        public async ValueTask<ItemCapData> GetGameCap(string name)
        {
            var contract = await GetContract(name);
            return new ItemCapData
            {
                CapObject = contract?.GameAdminCaps.SingleOrDefault(x => x.Name == name)?.Id,
                PackageId = contract?.PackageId
            };
        }

        public async ValueTask<ItemCapData> GetTreasuryCap(string name)
        {
            var contract = await GetContract(name);
            return new ItemCapData
            {
                CapObject = contract?.TreasuryCaps.SingleOrDefault(x => x.Name == name)?.Id,
                PackageId = contract?.PackageId
            };
        }
    }
}