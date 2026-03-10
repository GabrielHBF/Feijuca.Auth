using Feijuca.Auth.Application.Requests.User;
using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.User
{
    public record ChangePasswordCommand(string Tenant, ChangePasswordRequest ChangePasswordRequest) : ICommand<Result<bool>>;
}
