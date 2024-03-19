## Beamable SUI Sample project Runtime structure

Beamable SUI Sample project Runtime contains the microservice, storage object and common data.
```
├── Client (microservice auto-generated client)
├── Common (data shared between the microservice and Unity)
├── StorageObject (object definig access to the Mongo database)
├── SuiFederation (microservice)
```

### Auto-generated client
The Client folder contains auto-generated code, the microservice client, which can be used to call the microservice exposed methods from Unity.

### Common
The Common folder contains data, classes that are shared between the microservice and Unity.  
Example for this can be content type or federated identity definitions.
Federated identity definition example:
```
namespace Beamable.Sui.Common
{
    public class SuiCloudIdentity : IThirdPartyCloudIdentity
    {
        public string UniqueName => "sui";
    }
}
```

### StorageObject
The StorageObject folder contains Beamable Storage tool that enables you to manage and deploy a Mongo database from within Unity.  
This storage object is used by the microservice to store wallets, smart contracts, transactions and minted assets. 

## Beamable SUI Microservice structure
SUI Microservice structure can be found [here](SuiFederation/Readme.md).