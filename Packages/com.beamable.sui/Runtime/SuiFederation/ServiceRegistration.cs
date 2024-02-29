using System.Linq;
using System.Reflection;
using Beamable.Common.Dependencies;
using Beamable.Microservices.SuiFederation.Features.Transactions;

namespace Beamable.Microservices.SuiFederation
{
    public static class ServiceRegistration
    {
        public static void AddFeatures(this IDependencyBuilder builder)
        {
            Assembly.GetExecutingAssembly()
                .GetDerivedTypes<IService>()
                .ToList()
                .ForEach(serviceType => builder.AddSingleton(serviceType));

            builder.AddScoped<TransactionManager>();
        }
    }
}