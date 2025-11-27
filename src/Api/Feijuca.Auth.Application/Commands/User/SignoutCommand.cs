using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.User
{
    public record SignoutCommand(string RefreshToken) : ICommand<Result<bool>>;
}
