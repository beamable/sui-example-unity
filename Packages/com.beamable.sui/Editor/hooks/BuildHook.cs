using Beamable.Common.Dependencies;
using Beamable.Microservices.SuiFederation;
using Beamable.Server.Editor;

namespace Beamable.Editor.Sui.Hooks
{
    public class SuiFederationBuildHook : IMicroserviceBuildHook<SuiFederation>
    {
        const string SourceBasePath = "Packages/com.beamable.sui/Runtime/SuiFederation";
        
        public void Execute(IMicroserviceBuildContext ctx)
        {
            ctx.AddDirectory($"{SourceBasePath}/Solidity", "Solidity");
        }
    }
    
    [BeamContextSystem]
    public class Registrations
    {
        [RegisterBeamableDependencies(-1, RegistrationOrigin.EDITOR)]
        public static void Register(IDependencyBuilder builder)
        {
            builder.AddSingleton<IMicroserviceBuildHook<SuiFederation>, SuiFederationBuildHook>();
        }
    }
}
