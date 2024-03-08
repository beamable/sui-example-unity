using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Microservices.SuiFederation.Features.Accounts.Storage;
using Beamable.Microservices.SuiFederation.Features.Accounts.Storage.Models;
using Beamable.Microservices.SuiFederation.Features.Minting;
using Beamable.Microservices.SuiFederation.Features.SuiApi;
using Beamable.Microservices.SuiFederation.Features.SuiApi.Models;

namespace Beamable.Microservices.SuiFederation.Features.Accounts
{
    public class WalletService : IService
    {
        private readonly SuiApiService _suiApiServiceService;
        private readonly VaultCollection _vaultCollection;

        private SuiCapObjects _capObjects = new();

        public WalletService(SuiApiService suiApiServiceService, VaultCollection vaultCollection)
        {
            _suiApiServiceService = suiApiServiceService;
            _vaultCollection = vaultCollection;
        }

        public async Task InitializeObjects()
        {
            if (string.IsNullOrWhiteSpace(Configuration.PackageId) || string.IsNullOrWhiteSpace(Configuration.PrivateKey))
            {
                throw new ConfigurationException($"Couldn't initialize cap objects, publish the smart contract and store the packageId and private key in the realm config.");
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

        public async Task<SuiWallet> GetOrCreateWallet(string userId)
        {
            var wallet = await GetWallet(userId);
            if (wallet is null)
            {
                wallet = await CreateWallet(userId);
                BeamableLogger.Log("Saved wallet {walletName} -> {walletAddress}", userId, wallet?.Address);
            }
            return wallet;
        }

        private async Task<SuiWallet?> GetWallet(string userId)
        {
            var vault = await _vaultCollection.GetVaultByName(userId);
            if (vault is null)
            {
                return null;
            }

            //var privateKey = Rijndael.Decrypt(vault.PrivateKeyEncrypted, Configuration.RealmSecret, KeySize.Aes256);
            return new SuiWallet
            {
                Address = vault.AddressHex,
                PrivateKey = "" //privateKey
            };
        }

        private async Task<SuiWallet?> CreateWallet(string userId)
        {
            var suiKeys = await SuiApiService.ExportPrivateKey();
            var newWallet = new SuiWallet
            {
                Address = suiKeys.Public,
                PrivateKey = suiKeys.Private
            };

            return await _vaultCollection.TryInsertVault(new Vault
            {
                Name = userId,
                AddressHex = newWallet.Address,
                //PrivateKeyEncrypted = Rijndael.Encrypt(newWallet.PrivateKey, _configuration.RealmSecret, KeySize.Aes256)
            }) ? newWallet : null;
        }

        public async Task<bool> VerifySignature(string token, string challenge, string solution)
        {
            return await _suiApiServiceService.VerifySignature(token, challenge, solution);
        }

        public async Promise<FederatedInventoryProxyState> GetInventoryState(string id)
        {
            var coinBalance = await _suiApiServiceService.GetBalance(id, _capObjects.TreasuryCaps.Select(x => x.Name).ToArray());
            var suiObjects = await _suiApiServiceService.GetOwnedObjects(id);

            var items = new List<(string, FederatedItemProxy)>();
            var currencies = coinBalance.coins?.ToDictionary(coin => GetCurrencyContenId(coin.coinType), coin => coin.total);

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