using Beamable.Microservices.SuiFederation.Features.DistributedLock.Storage;

namespace Beamable.Microservices.SuiFederation.Features.DistributedLock
{
    public class LockManagerFactory : IService
    {
        private readonly LockCollection _lockCollection;

        public LockManagerFactory(LockCollection lockCollection)
        {
            _lockCollection = lockCollection;
        }

        public DistributedLockManager Create(string lockName)
        {
            return new DistributedLockManager(lockName, _lockCollection);
        }
    }
}