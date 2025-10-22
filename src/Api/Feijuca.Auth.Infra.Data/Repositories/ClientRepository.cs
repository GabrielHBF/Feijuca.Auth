using Feijuca.Auth.Common.Errors;
using Feijuca.Auth.Domain.Entities;
using Feijuca.Auth.Domain.Interfaces;
using Feijuca.Auth.Services;
using Flurl;
using Mattioli.Configurations.Models;
using Newtonsoft.Json;
using System.Text;

namespace Feijuca.Auth.Infra.Data.Repositories;

public class ClientRepository(IHttpClientFactory httpClientFactory,
    IAuthRepository _authRepository,
    ITenantService _tenantService) : BaseRepository(httpClientFactory), IClientRepository
{
    public async Task<bool> CreateClientAsync(ClientEntity client, CancellationToken cancellationToken)
    {
        var tokenDetails = await _authRepository.GetAccessTokenAsync(cancellationToken);

        using var httpClient = CreateHttpClientWithHeaders(tokenDetails.Data.Access_Token);
        var url = httpClient.BaseAddress
               .AppendPathSegment("admin")
               .AppendPathSegment("realms")
               .AppendPathSegment(_tenantService.Tenant.Name)
               .AppendPathSegment("clients");

        var clientConfig = new
        {
            clientId = client.ClientId,
            name = client.ClientId,
            description = client.Description,
            enabled = true,
            standardFlowEnabled = true,
            authorizationServicesEnabled = true,
            serviceAccountsEnabled = true,
            implicitFlowEnabled = true,
            directAccessGrantsEnabled = true,
            protocol = "openid-connect",
            redirectUris = client.RedirectUrls,
            publicClient = false
        };

        var jsonContent = JsonConvert.SerializeObject(clientConfig);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        using var response = await httpClient.PostAsync(url, content, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        return false;
    }

    public async Task<Result<ClientEntity>> GetClientAsync(string clientId, CancellationToken cancellationToken)
    {
        var tokenDetails = await _authRepository.GetAccessTokenAsync(cancellationToken);
        using var httpClient = CreateHttpClientWithHeaders(tokenDetails.Data.Access_Token);

        var url = httpClient.BaseAddress
                .AppendPathSegment("admin")
                .AppendPathSegment("realms")
                .AppendPathSegment(_tenantService.Tenant.Name)
                .AppendPathSegment("clients")
                .AppendQueryParam("clientId", clientId);

        using var response = await httpClient.GetAsync(url, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<IEnumerable<ClientEntity>>(responseContent)!;
            return Result<ClientEntity>.Success(result.First());
        }

        return Result<ClientEntity>.Failure(ClientErrors.GetClientsErrors);
    }

    public async Task<Result<IEnumerable<ClientEntity>>> GetClientsAsync(CancellationToken cancellationToken)
    {
        var tokenDetails = await _authRepository.GetAccessTokenAsync(cancellationToken);
        using var httpClient = CreateHttpClientWithHeaders(tokenDetails.Data.Access_Token);

        var url = httpClient.BaseAddress
                .AppendPathSegment("admin")
                .AppendPathSegment("realms")
                .AppendPathSegment(_tenantService.Tenant.Name)
                .AppendPathSegment("clients");

        using var response = await httpClient.GetAsync(url, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<IEnumerable<ClientEntity>>(responseContent)!;
            var defaultClients = new List<string> { "account", "admin-cli", "broker", "realm-management", "security-admin-console", "account-console" };
            var clientsWithoutDefaultClients = result.Where(client => !defaultClients.Contains(client.ClientId.ToString())).ToList();
            return Result<IEnumerable<ClientEntity>>.Success(clientsWithoutDefaultClients!);
        }

        return Result<IEnumerable<ClientEntity>>.Failure(ClientErrors.GetClientsErrors);
    }
}
