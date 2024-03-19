## Beamable SUI Federation microservice structure

Beamable SUI Federation microservice structure contains services for interacting with the SUI chain and Beamable services.
```
├── Caching (thread-safe in-memory caching service)
├── Endpoints (microservice exposed endpoints implementations)
├── Extensions (helper extenstions)
├── Features (services)
├── Lib (extrnal libraries used in the microservice)
├── Move (SUI smart contract templates, CLI tools)
```

### Microservice Features
Microservice Features folder contains services used for interacting with the SUI chain and Beamable services.

* Accounts - manages SUI wallet accounts creation and storage
* Contracts - manages SUI smart contract generation and storage
* DistributedLock - ensures that critical operations (eg. smart contract deployment) happens only once in the distributed environments
* ExecWrapper - installs a compatibility layer needed by the SUI CLI tool 
* Inventory - manages federated inventory functionality
* Minting - manages federated asset (items and currencies) minting
* SuiApi - a layer between C# and SUI TypeScript SDK. Enables invoking TypeScript functions from C#
* SuiClientWrapper - manages invocation of the SUI CLI tool and handles smart contract compile and deploy 
* Transactions - records minting transactions