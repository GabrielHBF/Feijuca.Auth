using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.User
{
    public record DeleteUserCommand(Guid Id) : ICommand<Result<bool>>;
}
