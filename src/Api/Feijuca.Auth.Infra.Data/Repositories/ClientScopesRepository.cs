using Feijuca.Auth.Common;
using Feijuca.Auth.Domain.Entities;
using Feijuca.Auth.Domain.Interfaces;
using Feijuca.Auth.Services;
using Flurl;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace Feijuca.Auth.Infra.Data.Repositories
{
    public class ClientScopesRepository(IHttpClientFactory httpClientFactory, IAuthRepository authRepository, ITenantService tenantService)
        : BaseRepository(httpClientFactory), IClientScopesRepository
    {
        public async Task<bool> AddAudienceMapperAsync(string clientScopeId, CancellationToken cancellationToken)
        {
            var tokenDetails = await authRepository.GetAccessTokenAsync(cancellationToken);
            using var httpClient = CreateHttpClientWithHeaders(tokenDetails.Data.Access_Token);

            var url = httpClient.BaseAddress
                .AppendPathSegment("admin")
                .AppendPathSegment("realms")
                .AppendPathSegment(tenantService.Tenant.Name)
                .AppendPathSegment("client-scopes")
                .AppendPathSegment($"{clientScopeId}")
                .AppendPathSegment($"protocol-mappers")
                .AppendPathSegment($"models");

            var audienceMapper = new
            {
                name = Constants.FeijucaApiClientName,
                protocol = "openid-connect",
                protocolMapper = "oidc-audience-mapper",
                config = new Dictionary<string, string>
                {
                    { "included.client.audience", Constants.FeijucaApiClientName },
                    { "id.token.claim", "true" },
                    { "access.token.claim", "true" },
                    { "claim.name", "aud" },
                    { "userinfo.token.claim", "false" },
                    { "access.token.introspection", "true" },
                    { "lightweight.access.token.claim", "false" }
                }
            };

            var response = await httpClient.PostAsJsonAsync(url, audienceMapper, cancellationToken: cancellationToken);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddUserPropertyMapperAsync(string clientScopeId, string userPropertyName, string claimName, CancellationToken cancellationToken)
        {
            var tokenDetails = await authRepository.GetAccessTokenAsync(cancellationToken);
            using var httpClient = CreateHttpClientWithHeaders(tokenDetails.Data.Access_Token);

            var url = httpClient.BaseAddress
                .AppendPathSegment("admin")
                .AppendPathSegment("realms")
                .AppendPathSegment(tenantService.Tenant.Name)
                .AppendPathSegment("client-scopes")
                .AppendPathSegment(clientScopeId)
                .AppendPathSegment("protocol-mappers")
                .AppendPathSegment("models");

            var userPropertyMapper = new
            {
                name = $"{claimName}-mapper",  // Nome amigável para o mapper
                protocol = "openid-connect",
                protocolMapper = "oidc-usermodel-attribute-mapper", //when wish create property type instead of attribute type use oidc-usermodel-property-mapper
                consentRequired = false,
                config = new Dictionary<string, string>
                {
                    { "user.attribute", userPropertyName },
                    { "claim.name", claimName },
                    { "jsonType.label", "String" },
                    { "id.token.claim", "true" },
                    { "access.token.claim", "true" },
                    { "userinfo.token.claim", "true" }
                }
            };

            var response = await httpClient.PostAsJsonAsync(url, userPropertyMapper, cancellationToken);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddClientScopesAsync(ClientScopeEntity clientScopesEntity, string tenant, CancellationToken cancellationToken)
        {
            var tokenDetails = await authRepository.GetAccessTokenAsync(cancellationToken);
            using var httpClient = CreateHttpClientWithHeaders(tokenDetails.Data.Access_Token);

            var url = httpClient.BaseAddress
                .AppendPathSegment("admin")
                .AppendPathSegment("realms")
                .AppendPathSegment(tenant)
                .AppendPathSegment("client-scopes");

            var clientScope = new
            {
                name = clientScopesEntity.Name,
                description = clientScopesEntity.Description,
                protocol = "openid-connect",
                attributes = new Dictionary<string, bool>
                {
                    { "display.on.consent.screen", true },
                    { "include.in.token.scope", true }
                },
                defaultScope = true
            };

            var jsonContent = JsonConvert.SerializeObject(clientScope);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            using var response = await httpClient.PostAsync(url, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> AddClientScopeToClientAsync(string clientId,
            string clientScopeId,
            bool isOptional,
            CancellationToken cancellationToken)
        {
            var tokenDetails = await authRepository.GetAccessTokenAsync(cancellationToken);
            using var httpClient = CreateHttpClientWithHeaders(tokenDetails.Data.Access_Token);

            var url = httpClient.BaseAddress
                .AppendPathSegment("admin")
                .AppendPathSegment("realms")
                .AppendPathSegment(tenantService.Tenant.Name)
                .AppendPathSegment("clients")
                .AppendPathSegment(clientId)
                .AppendPathSegment(isOptional ? "optional-client-scopes" : "default-client-scopes")
                .AppendPathSegment(clientScopeId);

            using var response = await httpClient.PutAsync(url, default, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<ClientScopeEntity>> GetClientScopesAsync(CancellationToken cancellationToken)
        {
            var tokenDetails = await authRepository.GetAccessTokenAsync(cancellationToken);
            using var httpClient = CreateHttpClientWithHeaders(tokenDetails.Data.Access_Token);

            var url = httpClient.BaseAddress
                .AppendPathSegment("admin")
                .AppendPathSegment("realms")
                .AppendPathSegment(tenantService.Tenant.Name)
                .AppendPathSegment("client-scopes");

            using var response = await httpClient.GetAsync(url, cancellationToken);
            var scopesList = new List<ClientScopeEntity>();

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                var clientScopes = JsonConvert.DeserializeObject<IEnumerable<ClientScopeEntity>>(responseBody);

                return clientScopes!;
            }

            return scopesList;

        }
    }
}
