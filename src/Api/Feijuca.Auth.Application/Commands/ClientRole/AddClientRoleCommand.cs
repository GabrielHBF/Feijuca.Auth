using Mattioli.Configurations.Models;
using Feijuca.Auth.Application.Requests.Role;
using LiteBus.Commands.Abstractions;

namespace Feijuca.Auth.Application.Commands.ClientRole
{
    public record AddClientRoleCommand(IEnumerable<AddClientRoleRequest> AddClientRolesRequest) : ICommand<Result<bool>>;
}
