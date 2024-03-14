using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Microservices.SuiFederation.Features.Minting.Storage.Models;
using Beamable.Server;
using MongoDB.Driver;

namespace Beamable.Microservices.SuiFederation.Features.Minting.Storage
{
    public class MintCollection : IService
    {
        private readonly IStorageObjectConnectionProvider _storageObjectConnectionProvider;
        private IMongoCollection<Mint>? _collection;

        public MintCollection(IStorageObjectConnectionProvider storageObjectConnectionProvider)
        {
            _storageObjectConnectionProvider = storageObjectConnectionProvider;
        }

        private async ValueTask<IMongoCollection<Mint>> Get()
        {
            if (_collection is null)
            {
                _collection =
                    (await _storageObjectConnectionProvider.SuiStorageDatabase()).GetCollection<Mint>("mint");
                await _collection.Indexes.CreateManyAsync(new[]
                {
                    new CreateIndexModel<Mint>(Builders<Mint>.IndexKeys.Ascending(x => x.PackageId),
                        new CreateIndexOptions
                        {
                            Name = "packageId"
                        }),
                    new CreateIndexModel<Mint>(Builders<Mint>.IndexKeys.Ascending(x => x.Owner),
                        new CreateIndexOptions
                        {
                            Name = "owner",
                        }),
                });
            }
            return _collection;
        }

        public async Task InsertMints(IEnumerable<Mint> mints)
        {
            var collection = await Get();
            var options = new InsertManyOptions
            {
                IsOrdered = false
            };
            try
            {
                await collection.InsertManyAsync(mints, options);
            }
            catch (MongoBulkWriteException e) when (e.WriteErrors.All(x => x.Category == ServerErrorCategory.DuplicateKey))
            {
                // Ignore
            }
        }
    }
}