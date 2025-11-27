using Mattioli.Configurations.Models;
using Feijuca.Auth.Application.Requests.GroupRoles;
using LiteBus.Commands.Abstractions;

namespace Feijuca.Auth.Application.Commands.GroupRoles
{
    public record AddClientRoleToGroupCommand(string GroupId, AddClientRoleToGroupRequest AddRoleToGroupRequest) : ICommand<Result<bool>>;
}
