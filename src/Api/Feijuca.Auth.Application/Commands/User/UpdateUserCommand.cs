using Feijuca.Auth.Application.Requests.User;
using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.User
{
    public record UpdateUserCommand(Guid Id, AddUserRequest UserRequest) : ICommand<Result<bool>>;
}
