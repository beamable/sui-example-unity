using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Microservices.SuiFederation.Features.Accounts.Models;
using Beamable.Microservices.SuiFederation.Features.Accounts.Storage;
using Beamable.Microservices.SuiFederation.Features.Accounts.Storage.Models;
using Beamable.Microservices.SuiFederation.Features.Minting;
using Beamable.Microservices.SuiFederation.Features.SuiApi;
using Beamable.Microservices.SuiFederation.Features.SuiApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Rijndael256;
using Rijndael = Rijndael256.Rijndael;

namespace Beamable.Microservices.SuiFederation.Features.Accounts
{
    public class AccountsService : IService
    {
        private const string RealmAccountName = "default-account";
        private readonly SuiApiService _suiApiServiceService;
        private readonly VaultCollection _vaultCollection;
        private Account? _cachedRealmAccount;
        private readonly MemoryCache _accountCache = new(Options.Create(new MemoryCacheOptions()));

        private SuiCapObjects _capObjects = new();

        public AccountsService(SuiApiService suiApiServiceService, VaultCollection vaultCollection)
        {
            _suiApiServiceService = suiApiServiceService;
            _vaultCollection = vaultCollection;
        }

        public async Task<Account> GetOrCreateAccount(string accountName)
        {
            var account = await GetAccount(accountName);
            if (account is null)
            {
                account = await CreateAccount(accountName);
                if (account is null)
                {
                    BeamableLogger.LogWarning("Account already created, fetching again");
                    return await GetOrCreateAccount(accountName);
                }

                BeamableLogger.Log("Saved account {accountName} -> {accountAddress}", accountName, account.Address);
            }

            return account;
        }

        public async ValueTask<Account> GetOrCreateRealmAccount()
        {
            if (_cachedRealmAccount is not null)
                return _cachedRealmAccount;

            var account = await GetAccount(RealmAccountName);
            if (account is null)
            {
                account = await CreateAccount(RealmAccountName);
                if (account is null)
                {
                    BeamableLogger.LogWarning("Account already created, fetching again");
                    return await GetOrCreateRealmAccount();
                }

                BeamableLogger.Log("Saved account {accountName} -> {accountAddress}", RealmAccountName,
                    account.Address);
                BeamableLogger.LogWarning(
                    "Please add some gas money to your account {accountAddress} to be able to pay for fees.",
                    account.Address);
            }

            _cachedRealmAccount = account;
            return account;
        }

        public async Task InitializeObjects()
        {
            if (string.IsNullOrWhiteSpace(Configuration.PackageId) ||
                string.IsNullOrWhiteSpace(Configuration.PrivateKey))
            {
                throw new ConfigurationException(
                    $"Couldn't initialize cap objects, publish the smart contract and store the packageId and private key in the realm config.");
            }

            var caps = await _suiApiServiceService.InitializeObjects();
            _capObjects = caps;
            BeamableLogger.Log("Initialized CAP objects for package {PackageId}", Configuration.PackageId);
        }

        public SuiCapObject? GetGameCap(string name)
        {
            return _capObjects.GameAdminCaps.SingleOrDefault(x => x.Name == name);
        }

        public SuiCapObject? GetTreasuryCap(string name)
        {
            return _capObjects.TreasuryCaps.SingleOrDefault(x => x.Name == name);
        }

        private async Task<Account?> GetAccount(string accountName)
        {
            return await _accountCache.GetOrCreateAsync(accountName, async cacheEntry =>
            {
                var vault = await _vaultCollection.GetVaultByName(accountName);
                if (vault is null)
                {
                    cacheEntry.Dispose();
                    return null;
                }
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(5);
                var privateKey = Rijndael.Decrypt(vault.PrivateKeyEncrypted, Configuration.RealmSecret, KeySize.Aes256);
                return new Account
                {
                    Name = vault.Name,
                    Address = vault.AddressHex,
                    PrivateKey = privateKey
                };
            });
        }

        private async Task<Account?> CreateAccount(string accountName)
        {
            var suiKeys = await SuiApiService.ExportPrivateKey();
            var privateKeyEncrypted = Rijndael.Encrypt(suiKeys.Private, Configuration.RealmSecret, KeySize.Aes256);
            var newAccount = new Account
            {
                Name = accountName,
                Address = suiKeys.Public,
                PrivateKey = suiKeys.Private
            };

            return await _vaultCollection.TryInsertVault(new Vault
            {
                Name = accountName,
                AddressHex = newAccount.Address,
                PrivateKeyEncrypted = privateKeyEncrypted
            })
                ? newAccount
                : null;
        }

        public async Task<bool> VerifySignature(string token, string challenge, string solution)
        {
            return await _suiApiServiceService.VerifySignature(token, challenge, solution);
        }

        public async Promise<FederatedInventoryProxyState> GetInventoryState(string id)
        {
            var coinBalance =
                await _suiApiServiceService.GetBalance(id, _capObjects.TreasuryCaps.Select(x => x.Name).ToArray());
            var suiObjects = await _suiApiServiceService.GetOwnedObjects(id);

            var items = new List<(string, FederatedItemProxy)>();
            var currencies =
                coinBalance.coins?.ToDictionary(coin => GetCurrencyContenId(coin.coinType), coin => coin.total);

            foreach (var suiObject in suiObjects)
            {
                items.Add((GetItemContenId(suiObject.type),
                        new FederatedItemProxy
                        {
                            proxyId = suiObject.objectId,
                            properties = suiObject.GetProperties().ToList()
                        }
                    ));
            }

            var itemGroups = items
                .GroupBy(i => i.Item1)
                .ToDictionary(g => g.Key, g => g.Select(i => i.Item2).ToList());

            return new FederatedInventoryProxyState
            {
                currencies = currencies,
                items = itemGroups
            };
        }

        private string GetItemContenId(string itemName)
        {
            return $"items.blockchain_item.{itemName}";
        }

        private string GetCurrencyContenId(string currencyName)
        {
            return $"currency.blockchain_currency.{currencyName}";
        }
    }
}