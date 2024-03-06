using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Content;
using Beamable.Microservices.SuiFederation.Features.Accounts;
using Beamable.Microservices.SuiFederation.Features.Contracts;
using Beamable.Microservices.SuiFederation.Features.Minting.Models;
using Beamable.Microservices.SuiFederation.Features.SuiApi;
using Beamable.Microservices.SuiFederation.Features.Transactions;
using Beamable.Microservices.SuiFederation.Features.Transactions.Storage.Models;
using Beamable.Sui.Common.Content;

namespace Beamable.Microservices.SuiFederation.Features.Minting
{
    public class MintingService : IService
    {
        private readonly ContentService _contentService;
        private readonly SuiApiService _suiApiServiceService;
        private readonly TransactionManager _transactionManager;
        private readonly AccountsService _accountsService;
        private readonly ContractProxy _contractProxy;

        public MintingService(ContentService contentService, SuiApiService suiApiServiceService,
            TransactionManager transactionManager, AccountsService accountsService, ContractProxy contractProxy)
        {
            _contentService = contentService;
            _suiApiServiceService = suiApiServiceService;
            _transactionManager = transactionManager;
            _accountsService = accountsService;
            _contractProxy = contractProxy;
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
                        var treasuryCap = await _contractProxy.GetTreasuryCap(currencyItem.Name);
                        if (treasuryCap is not null)
                        {
                            currencyItem.TreasuryCap = treasuryCap;
                            mintRequest.CurrencyItems.Add(currencyItem);
                        }

                        break;
                    case BlockchainItem blockchainItem:
                        var inventoryItem = request.ToGameItem(blockchainItem);
                        var gameCap = await _contractProxy.GetGameCap(inventoryItem.ContentName);
                        if (gameCap is not null)
                        {
                            inventoryItem.GameAdminCap = gameCap;
                            mintRequest.GameItems.Add(inventoryItem);
                        }

                        break;
                    default:
                        throw new UndefinedItemException(nameof(contentDefinition));
                }
            }

            var contract = await _contractProxy.GetDefaultContract();
            var account = await _accountsService.GetOrCreateRealmAccount();

            var result = await _suiApiServiceService.MintInventoryItems(toWalletAddress, mintRequest, contract, account);
            if (result.error is null)
            {
                await _transactionManager.MarkConfirmed(inventoryTransactionId);
            }
            else
            {
                await _transactionManager.MarkFailed(inventoryTransactionId);
            }
        }
    }
}