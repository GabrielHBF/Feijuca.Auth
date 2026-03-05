using Mattioli.Configurations.Models;
using Feijuca.Auth.Application.Requests.GroupRoles;
using LiteBus.Commands.Abstractions;

namespace Feijuca.Auth.Application.Commands.GroupRoles
{
    public record RemoveRoleFromGroupCommand(string GroupId, RemoveClientRoleToGroupRequest RemoveRoleFromGroupRequest) : ICommand<Result<bool>>;
}
