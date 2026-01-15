using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using Feijuca.Auth.Providers;
using Flurl;

namespace Feijuca.Auth.Infra.Data.Repositories
{
    public class UserGroupRepository(IHttpClientFactory httpClientFactory, 
        IAuthRepository _authRepository, 
        ITenantProvider _tenantService) : BaseRepository(httpClientFactory), IGroupUsersRepository
    {
        public async Task<Result<bool>> AddUserToGroupAsync(Guid userId, string tenant, Guid groupId, CancellationToken cancellationToken)
        {
            var tokenDetails = await _authRepository.GetAccessTokenAsync(cancellationToken);
            using var httpClient = CreateHttpClientWithHeaders(tokenDetails.Data.Access_Token);

            var url = httpClient.BaseAddress
                    .AppendPathSegment("admin")
                    .AppendPathSegment("realms")
                    .AppendPathSegment(tenant)
                    .AppendPathSegment("users")
                    .AppendPathSegment(userId)
                    .AppendPathSegment("groups")
                    .AppendPathSegment(groupId);

            var response = await httpClient.PutAsync(url, default, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure(UserGroupErrors.ErrorAddUserToGroup);
        }

        public async Task<Result<bool>> RemoveUserFromGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
        {
            var tokenDetails = await _authRepository.GetAccessTokenAsync(cancellationToken);
            using var httpClient = CreateHttpClientWithHeaders(tokenDetails.Data.Access_Token);

            var url = httpClient.BaseAddress
                    .AppendPathSegment("admin")
                    .AppendPathSegment("realms")
                    .AppendPathSegment(_tenantService.Tenant.Name)
                    .AppendPathSegment("users")
                    .AppendPathSegment(userId)
                    .AppendPathSegment("groups")
                    .AppendPathSegment(groupId);

            using var response = await httpClient.DeleteAsync(url, default);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure(UserGroupErrors.ErrorAddUserToGroup);
        }
    }
}
