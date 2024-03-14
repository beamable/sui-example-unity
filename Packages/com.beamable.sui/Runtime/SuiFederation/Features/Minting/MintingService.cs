using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Microservices.SuiFederation.Features.Accounts;
using Beamable.Microservices.SuiFederation.Features.Contracts;
using Beamable.Microservices.SuiFederation.Features.Minting.Models;
using Beamable.Microservices.SuiFederation.Features.Minting.Storage;
using Beamable.Microservices.SuiFederation.Features.Minting.Storage.Models;
using Beamable.Microservices.SuiFederation.Features.SuiApi;
using Beamable.Microservices.SuiFederation.Features.Transactions;
using Beamable.Microservices.SuiFederation.Features.Transactions.Storage.Models;
using Beamable.Server.Api.Content;
using Beamable.Sui.Common.Content;

namespace Beamable.Microservices.SuiFederation.Features.Minting
{
    public class MintingService : IService
    {
        private readonly IMicroserviceContentApi _contentService;
        private readonly SuiApiService _suiApiServiceService;
        private readonly TransactionManager _transactionManager;
        private readonly AccountsService _accountsService;
        private readonly ContractProxy _contractProxy;
        private readonly MintCollection _mintCollection;


        public MintingService(IMicroserviceContentApi contentService, SuiApiService suiApiServiceService,
            TransactionManager transactionManager, AccountsService accountsService, ContractProxy contractProxy, MintCollection mintCollection)
        {
            _contentService = contentService;
            _suiApiServiceService = suiApiServiceService;
            _transactionManager = transactionManager;
            _accountsService = accountsService;
            _contractProxy = contractProxy;
            _mintCollection = mintCollection;
        }

        public async Task Mint(long userId, string toWalletAddress, string inventoryTransactionId,
            ICollection<MintRequestData> requests)
        {
            var existingTransaction = await _transactionManager.GetTransaction(inventoryTransactionId);
            if (existingTransaction is not null && existingTransaction.State == TransactionState.Confirmed)
            {
                return;
            }

            var mintRequest = new InventoryMintRequest();

            foreach (var request in requests)
            {
                var contentDefinition = await _contentService.GetContent(request.ContentId);

                switch (contentDefinition)
                {
                    case BlockchainCurrency blockchainCurrency:
                        var currencyItem = request.ToCurrencyItem(blockchainCurrency);
                        var treasuryCap = await _contractProxy.GetTreasuryCap(request.ContentId);
                        if (treasuryCap is not null)
                        {
                            currencyItem.TreasuryCap = treasuryCap.CapObject;
                            currencyItem.PackageId = treasuryCap.PackageId;
                            mintRequest.CurrencyItems.Add(currencyItem);
                        }
                        break;
                    case BlockchainItem blockchainItem:
                        var inventoryItem = request.ToGameItem(blockchainItem, request.Properties);
                        var gameCap = await _contractProxy.GetGameCap(request.ContentId);
                        if (gameCap is not null)
                        {
                            inventoryItem.GameAdminCap = gameCap.CapObject;
                            inventoryItem.PackageId = gameCap.PackageId;
                            mintRequest.GameItems.Add(inventoryItem);
                        }
                        break;
                    default:
                        throw new UndefinedItemException(nameof(contentDefinition));
                }
            }

            var account = await _accountsService.GetOrCreateRealmAccount();

            var result = await _suiApiServiceService.MintInventoryItems(toWalletAddress, mintRequest, account);
            if (result.error is null)
            {
                await _mintCollection.InsertMints(mintRequest.ToMints(toWalletAddress));
                await _transactionManager.MarkConfirmed(inventoryTransactionId);
            }
            else
            {
                await _transactionManager.MarkFailed(inventoryTransactionId);
            }
        }
    }
}