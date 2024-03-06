using System.Threading.Tasks;
using Beamable.Microservices.SuiFederation.Features.SuiApi;

namespace Beamable.Microservices.SuiFederation.Features.Accounts
{
    public class AuthenticationService : IService
    {
        private readonly SuiApiService _suiApiService;

        public AuthenticationService(SuiApiService suiApiService)
        {
            _suiApiService = suiApiService;
        }

        public async Task<bool> IsSignatureValid(string token, string challenge, string solution)
        {
            return await _suiApiService.VerifySignature(token, challenge, solution);
        }
    }
}