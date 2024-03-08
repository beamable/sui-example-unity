﻿using System.Threading.Tasks;
using Beamable.Microservices.SuiFederation.Features.Minting.Storage.Models;
using MongoDB.Driver;

namespace Beamable.Microservices.SuiFederation.Features.Minting.Storage
{
    internal static class CounterCollection
    {
        private static IMongoCollection<Counter> _collection;

        private static IMongoCollection<Counter> Get(IMongoDatabase db)
        {
            if (_collection is null)
            {
                _collection = db.GetCollection<Counter>("counter");
            }

            return _collection;
        }

        public static async Task<uint> GetNextCounterValue(this IMongoDatabase db, string counterName)
        {
            var collection = Get(db);
            var update = Builders<Counter>.Update.Inc(x => x.State, (uint)1);

            var options = new FindOneAndUpdateOptions<Counter>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };

            var updated = await collection.FindOneAndUpdateAsync<Counter>(x => x.Name == counterName, update, options);

            return updated.State - 1;
        }
    }
}