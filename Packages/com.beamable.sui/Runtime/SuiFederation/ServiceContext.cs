using System;
using Beamable.Common.Api;
using Beamable.Microservices.SuiFederation.Features.Contracts.Storage.Models;
using Beamable.Microservices.SuiFederation.Features.EthRpc;
using MongoDB.Driver;
using Nethereum.Web3.Accounts;

namespace Beamable.Microservices.SuiFederation
{
    internal static class ServiceContext
    {
        public static IMongoDatabase Database;
        public static Account RealmAccount;
        public static Contract DefaultContract;
        public static EthRpcClient RpcClient;
        public static IBeamableRequester Requester;
    }
}