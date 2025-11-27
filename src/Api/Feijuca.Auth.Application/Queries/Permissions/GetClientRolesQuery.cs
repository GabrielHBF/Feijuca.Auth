using Mattioli.Configurations.Models;
using Feijuca.Auth.Application.Responses;
using LiteBus.Queries.Abstractions;

namespace Feijuca.Auth.Application.Queries.Permissions
{
    public record GetClientRolesQuery() : IQuery<Result<IEnumerable<ClientRoleResponse>>>;
}
