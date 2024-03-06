using System;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Microservices.SuiFederation.Features.Accounts.Models;
using Beamable.Microservices.SuiFederation.Features.Accounts.Storage;
using Beamable.Microservices.SuiFederation.Features.Accounts.Storage.Models;
using Beamable.Microservices.SuiFederation.Features.Contracts;
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
        private readonly ContractProxy _contractProxy;
        private readonly VaultCollection _vaultCollection;
        private Account? _cachedRealmAccount;
        private readonly MemoryCache _accountCache = new(Options.Create(new MemoryCacheOptions()));

        private SuiCapObjects _capObjects = new();

        public AccountsService(VaultCollection vaultCollection, SuiApiService suiApiServiceService, ContractProxy contractProxy)
        {
            _vaultCollection = vaultCollection;
            _suiApiServiceService = suiApiServiceService;
            _contractProxy = contractProxy;
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
            BeamableLogger.Log($"Generated realm-account {suiKeys.Public} - {suiKeys.Private}");
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
    }
}