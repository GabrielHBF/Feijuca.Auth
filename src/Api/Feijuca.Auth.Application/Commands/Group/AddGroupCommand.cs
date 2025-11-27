using Mattioli.Configurations.Models;
using Feijuca.Auth.Application.Requests.User;
using LiteBus.Commands.Abstractions;

namespace Feijuca.Auth.Application.Commands.Group
{
    public record AddGroupCommand(AddGroupRequest AddGroupRequest) : ICommand<Result<bool>>;
}
