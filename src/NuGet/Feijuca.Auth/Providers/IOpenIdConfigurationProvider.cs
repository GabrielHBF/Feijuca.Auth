using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Feijuca.Auth.Providers;

public interface IOpenIdConfigurationProvider
{
    public Task<OpenIdConnectConfiguration> GetAsync(string issuer, CancellationToken cancellation);
    public void RequestRefresh(string issuer);
}