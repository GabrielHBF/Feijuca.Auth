using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Concurrent;

namespace Feijuca.Auth.Providers
{
    public class OpenIdConfigurationProvider : IOpenIdConfigurationProvider
    {
        private readonly ConcurrentDictionary<string, ConfigurationManager<OpenIdConnectConfiguration>> _managers = new();
        private readonly TimeSpan _automaticRefreshInterval;
        private readonly TimeSpan _refreshInterval;

        public OpenIdConfigurationProvider(TimeSpan? automaticRefreshInterval = null, TimeSpan? refreshInterval = null)
        {
            _automaticRefreshInterval = automaticRefreshInterval ?? TimeSpan.FromHours(12);
            _refreshInterval = refreshInterval ?? TimeSpan.FromMinutes(1);
        }

        public Task<OpenIdConnectConfiguration> GetAsync(string issuer, CancellationToken cancellationToken)
        {
            var maneger = _managers.GetOrAdd(issuer, CreateManager);

            return maneger.GetConfigurationAsync(cancellationToken);
        }

        public void RequestRefresh(string issuer)
        {
            if (_managers.TryGetValue(issuer, out var manager))
            {
                manager.RequestRefresh();
            }
        }

        private ConfigurationManager<OpenIdConnectConfiguration> CreateManager(string issuer)
        {
            var metadataAddress = $"{issuer.TrimEnd('/')}/.well-known/openid-configuration";

            return new ConfigurationManager<OpenIdConnectConfiguration>(
                metadataAddress,
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever { RequireHttps = true })
            {
                AutomaticRefreshInterval = _automaticRefreshInterval,
                RefreshInterval = _refreshInterval
            };
        }
    }
}