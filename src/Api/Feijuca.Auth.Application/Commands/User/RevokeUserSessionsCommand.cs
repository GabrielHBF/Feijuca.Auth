using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.User
{
    public record RevokeUserSessionsCommand(Guid UserId) : ICommand<Result>;
}
