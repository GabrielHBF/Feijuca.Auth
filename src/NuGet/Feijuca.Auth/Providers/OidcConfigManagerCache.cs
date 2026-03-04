using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Concurrent;

namespace Feijuca.Auth.Providers
{
    public sealed class OidcConfigManagerCache : IOidcConfigManagerCache
    {
        private readonly ConcurrentDictionary<string, IConfigurationManager<OpenIdConnectConfiguration>> _cache = new();

        public IConfigurationManager<OpenIdConnectConfiguration> Get(string metadataAddress, bool requireHttps)
        {
            return _cache.GetOrAdd(metadataAddress, addr =>
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    addr,
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever { RequireHttps = requireHttps })
                {
                    AutomaticRefreshInterval = TimeSpan.FromHours(12),
                    RefreshInterval = TimeSpan.FromMinutes(5)
                });
        }
    }
}
