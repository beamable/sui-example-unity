namespace Beamable.Microservices.SuiFederation.Features.Accounts.Models
{
    public class Account
    {
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string PrivateKey { get; set; } = default!;
    }
}