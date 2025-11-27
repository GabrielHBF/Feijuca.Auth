using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.Config
{
    public record UpdateFeijucaConfigWithClientIdAndSecretCommandHandler : ICommand<Result<bool>>;
}
