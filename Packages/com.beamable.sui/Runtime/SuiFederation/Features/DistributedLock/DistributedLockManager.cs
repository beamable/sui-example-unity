using System.Threading.Tasks;
using Beamable.Microservices.SuiFederation.Features.DistributedLock.Storage;

namespace Beamable.Microservices.SuiFederation.Features.DistributedLock
{
    public class DistributedLockManager
    {
        private readonly LockCollection _lockCollection;
        private readonly string _lockName;

        public DistributedLockManager(string lockName, LockCollection lockCollection)
        {
            _lockCollection = lockCollection;
            _lockName = lockName;
        }

        public async Task<bool> AcquireLock(int lockTimeoutSeconds)
        {
            return await _lockCollection.AcquireLock(_lockName, lockTimeoutSeconds);
        }

        public async Task ReleaseLock()
        {
            await _lockCollection.ReleaseLock(_lockName);
        }
    }
}