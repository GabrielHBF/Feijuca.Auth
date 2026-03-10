using Feijuca.Auth.Application.Requests.User;
using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.User
{
    public record ResetPasswordCommand(ResetPasswordRequest ChangePasswordRequest) : ICommand<Result<bool>>;
}
