using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.GroupUser
{
    public record AddUserToGroupCommand(Guid UserId, Guid GroupId) : ICommand<Result<bool>>;
}
