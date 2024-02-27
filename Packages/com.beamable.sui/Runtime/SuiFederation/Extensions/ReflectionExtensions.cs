using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Beamable.Microservices.SuiFederation
{

    public static class ReflectionExtensions
    {
        private static readonly Func<Assembly, Type[]> AssemblyTypesMemo = Memoizer.Memoize((Assembly assembly) => assembly.GetTypes());

        public static IEnumerable<Type> GetDerivedTypes<TBase>(this Assembly assembly)
        {
            return AssemblyTypesMemo(assembly).Where(type => typeof(TBase).IsAssignableFrom(type) && !type.IsInterface);
        }
    }
}