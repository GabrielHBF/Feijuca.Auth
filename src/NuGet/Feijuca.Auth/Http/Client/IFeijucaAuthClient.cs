using Mattioli.Configurations.Http;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Http.Responses;

namespace Feijuca.Auth.Http.Client;

public interface IFeijucaAuthClient
{
    Task<Result<TokenDetailsResponse>> AuthenticateUserAsync(string username, string password, CancellationToken cancellationToken);
    Task<Result<PagedResult<UserResponse>>> GetUsersAsync(int maxUsers, string jwtToken, CancellationToken cancellationToken);
    Task<Result<UserResponse>> GetUserAsync(string userame, string jwtToken, CancellationToken cancellationToken);
    Task<Result<IEnumerable<GroupResponse>>> GetGroupsAsync(string jwtToken, CancellationToken cancellationToken);
    Task<Result<IEnumerable<RealmResponse>>> GetRealmsAsync(string jwtToken, CancellationToken cancellationToken);
}
