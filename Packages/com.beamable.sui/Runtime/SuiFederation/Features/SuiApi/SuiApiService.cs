using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Beamable.Microservices.SuiFederation.Features.EthRpc;
using Beamable.Common;
using Beamable.Microservices.SuiFederation.Features.Minting.Models;
using Beamable.Microservices.SuiFederation.Features.SuiApi.Exceptions;
using Beamable.Microservices.SuiFederation.Features.SuiApi.Models;
using Newtonsoft.Json;

namespace Beamable.Microservices.SuiFederation.Features.SuiApi
{
    public class SuiApiService : IService
    {
        private const string BridgeModulePath = "./js/bridge.js";

        public static async Task<SuiKeys> ExportPrivateKey()
        {
            using (new Measure($"Sui.exportPrivateKey"))
            {
                try
                {
                    // var response = await StaticNodeJSService.InvokeFromFileAsync<string>(
                    //     BridgeModulePath,
                    //     "exportSecret");
                    // return JsonConvert.DeserializeObject<SuiKeys>(response);
                    return new SuiKeys();
                }
                catch (Exception ex)
                {
                    BeamableLogger.LogWarning("Can't generate new private key. Error: {error}", ex.Message);
                    throw new SuiApiException(ex.Message);
                }
            }
        }

        public async Task<bool> VerifySignature(string token, string challenge, string solution)
        {
            using (new Measure($"Sui.verifySignature: {token}"))
            {
                try
                {
                    // return await StaticNodeJSService.InvokeFromFileAsync<bool>(
                    //     BridgeModulePath,
                    //     "verifySignature",
                    //     new object[] { token, challenge, solution });
                    return true;
                }
                catch (Exception ex)
                {
                    BeamableLogger.LogWarning("Can't verify signature for {token}. Error: {error}", token, ex.Message);
                    throw new SuiApiException(ex.Message);
                }
            }
        }

        public async Task<SuiBalance> GetBalance(string address, string[] coinModules)
        {
            using (new Measure($"Sui.GetBalance: {address} for -> {string.Join(',', coinModules)}"))
            {
                try
                {
                    var environment = Configuration.SuiEnvironment;
                    var packageId = Configuration.PackageId;
                    // var response = await StaticNodeJSService.InvokeFromFileAsync<string>(
                    //     BridgeModulePath,
                    // "getBalance",
                    //     new object[] { address, packageId, coinModules, environment });
                    // return JsonConvert.DeserializeObject<SuiBalance>(response);
                    return new SuiBalance();
                }
                catch (Exception ex)
                {
                    BeamableLogger.LogWarning("Can't get balance for {address}. Error: {error}", address, ex.Message);
                    throw new SuiApiException(ex.Message);
                }
            }
        }

        public async Task<IEnumerable<SuiObject>> GetOwnedObjects(string address)
        {
            using (new Measure($"Sui.GetOwnedObjects: {address}"))
            {
                try
                {
                    var environment = Configuration.SuiEnvironment;
                    var packageId = Configuration.PackageId;
                    // var result = await StaticNodeJSService.InvokeFromFileAsync<string>(
                    //     BridgeModulePath,
                    //     "getOwnedObjects",
                    //     new object[] { address, packageId, environment });
                    //
                    // return JsonConvert.DeserializeObject<IEnumerable<SuiObject>>(result);
                    return Enumerable.Empty<SuiObject>();
                }
                catch (Exception ex)
                {
                    BeamableLogger.LogWarning("Can't get objects for {address}. Error: {error}", address, ex.Message);
                    throw new SuiApiException(ex.Message);
                }
            }
        }

        public async Task<SuiCapObjects> InitializeObjects()
        {
            using (new Measure("Sui.Initialize"))
            {
                try
                {
                    var environment = Configuration.SuiEnvironment;
                    var packageId = Configuration.PackageId;
                    var secretKey = Configuration.PrivateKey;
                    // var result = await StaticNodeJSService.InvokeFromFileAsync<string>(
                    //     BridgeModulePath,
                    //     "getCapObjects",
                    //     new object[] { secretKey, packageId, environment });
                    //
                    // return JsonConvert.DeserializeObject<SuiCapObjects>(result);
                    return new SuiCapObjects();
                }
                catch (Exception ex)
                {
                    BeamableLogger.LogWarning("Can't get cap objects. Error: {error}", ex.Message);
                    throw new SuiApiException(ex.Message);
                }
            }
        }

        public async Task<SuiTransactionResult> MintInventoryItems(string token, InventoryMintRequest request)
        {
            using (new Measure(
                       $"Sui.MintInventoryItems: for {token} -> {string.Join(',', request.GameItems.Select(x => x.Name))} -> Currency: {string.Join(',', request.CurrencyItems.Select(x => x.Name))}"))
            {
                try
                {
                    var environment = Configuration.SuiEnvironment;
                    var packageId = Configuration.PackageId;
                    var secretKey = Configuration.PrivateKey;
                    var mintRequestJson = JsonConvert.SerializeObject(request);
                    // var result =  await StaticNodeJSService.InvokeFromFileAsync<string>(
                    //     BridgeModulePath,
                    //     "mintInventory",
                    //     new object[] { packageId, token, mintRequestJson, secretKey, environment });
                    // return JsonConvert.DeserializeObject<SuiTransactionResult>(result);
                    return new SuiTransactionResult();
                }
                catch (Exception ex)
                {
                    BeamableLogger.LogWarning("Can't mint items for {token}. Error: {error}", token, ex.Message);
                    throw new SuiApiException(ex.Message);
                }
            }
        }
    }
}