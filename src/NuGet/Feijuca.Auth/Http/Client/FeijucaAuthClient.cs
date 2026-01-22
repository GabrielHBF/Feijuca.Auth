using Mattioli.Configurations.Http;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Errors;
using Feijuca.Auth.Http.Requests;
using Feijuca.Auth.Http.Responses;

namespace Feijuca.Auth.Http.Client
{
    public class FeijucaAuthClient(HttpClient httpClient) : BaseHttpClient(httpClient), IFeijucaAuthClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Result<TokenDetailsResponse>> AuthenticateUserAsync(string tenant, string username, string password, CancellationToken cancellationToken)
        {
            IncludeTenantHeader(tenant);

            var result = await PostAsync<LoginUserRequest, TokenDetailsResponse>("users/login", new LoginUserRequest(username, password), cancellationToken);

            if (string.IsNullOrEmpty(result.AccessToken))
            {
                return Result<TokenDetailsResponse>.Failure(FeijucaErrors.GenerateTokenError);
            }

            return Result<TokenDetailsResponse>.Success(result);
        }

        public async Task<Result<UserResponse>> GetUserAsync(string tenant, string userame, string jwtToken, CancellationToken cancellationToken)
        {
            IncludeTenantHeader(tenant);

            var url = $"users?Usernames={userame}";
            var result = await GetAsync<PagedResult<UserResponse>>(url, jwtToken, cancellationToken);

            if (result.TotalResults == 0)
            {
                return Result<UserResponse>.Failure(FeijucaErrors.GetUserErrors);
            }

            var user = result?.Results.FirstOrDefault(x => x.Email == userame);

            return Result<UserResponse>.Success(user!);
        }

        public async Task<Result<PagedResult<UserResponse>>> GetUsersAsync(string tenant, int maxUsers, string jwtToken, CancellationToken cancellationToken)
        {
            IncludeTenantHeader(tenant);

            var url = $"users?PageFilter.Page=1&PageFilter.PageSize={maxUsers}";
            var result = await GetAsync<PagedResult<UserResponse>>(url, jwtToken, cancellationToken);

            if (result.TotalResults <= 1)
            {
                return Result<PagedResult<UserResponse>>.Failure(FeijucaErrors.GetUserErrors);
            }

            return Result<PagedResult<UserResponse>>.Success(result);
        }

        public async Task<Result<IEnumerable<GroupResponse>>> GetGroupsAsync(string tenant, string jwtToken, CancellationToken cancellationToken)
        {
            IncludeTenantHeader(tenant);

            var result = await GetAsync<IEnumerable<GroupResponse>>("groups", jwtToken, cancellationToken);

            if (!result.Any())
            {
                return Result<IEnumerable<GroupResponse>>.Failure(FeijucaErrors.GetGroupErrors);
            }

            return Result<IEnumerable<GroupResponse>>.Success(result);
        }

        public async Task<Result<IEnumerable<RealmResponse>>> GetRealmsAsync(string jwtToken, CancellationToken cancellationToken)
        {
            var result = await GetAsync<IEnumerable<RealmResponse>>("realms", jwtToken, cancellationToken);

            if (!result.Any())
            {
                return Result<IEnumerable<RealmResponse>>.Failure(FeijucaErrors.GetGroupErrors);
            }

            return Result<IEnumerable<RealmResponse>>.Success(result);
        }

        private void IncludeTenantHeader(string tenant)
        {
            if (!string.IsNullOrEmpty(tenant))
            {
                if (_httpClient.DefaultRequestHeaders.Contains("Tenant"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Tenant");
                }

                _httpClient.DefaultRequestHeaders.Add("Tenant", tenant);
            }
        }
    }
}
