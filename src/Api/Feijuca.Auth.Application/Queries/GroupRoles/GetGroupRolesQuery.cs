using Mattioli.Configurations.Models;
using Feijuca.Auth.Application.Responses;
using LiteBus.Queries.Abstractions;

namespace Feijuca.Auth.Application.Queries.GroupRoles
{
    public record GetGroupRolesQuery(string GroupId) : IQuery<Result<IEnumerable<GroupRolesResponse>>>;
}
