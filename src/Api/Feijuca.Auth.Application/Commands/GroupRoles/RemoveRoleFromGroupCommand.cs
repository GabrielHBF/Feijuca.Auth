using Mattioli.Configurations.Models;
using Feijuca.Auth.Application.Requests.GroupRoles;
using LiteBus.Commands.Abstractions;

namespace Feijuca.Auth.Application.Commands.GroupRoles
{
    public record RemoveRoleFromGroupCommand(string GroupId, AddClientRoleToGroupRequest RemoveRoleFromGroupRequest) : ICommand<Result<bool>>;
}
