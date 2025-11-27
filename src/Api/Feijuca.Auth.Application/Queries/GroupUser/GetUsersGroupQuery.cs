using Mattioli.Configurations.Models;

using Feijuca.Auth.Application.Requests.GroupUsers;
using Feijuca.Auth.Application.Responses;
using LiteBus.Queries.Abstractions;

namespace Feijuca.Auth.Application.Queries.GroupUser
{
    public record GetUsersGroupQuery(GetUsersGroupRequest GetUsersGroupRequest) : IQuery<Result<PagedResult<UserGroupResponse>>>;
}
