using Feijuca.Auth.Common.Models;
using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.Config
{
    public record AddOrUpdateConfigCommand(KeycloakSettings Request) : ICommand<Result<bool>>;
}
