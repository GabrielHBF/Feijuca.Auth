using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.ClientScopeMapper
{
    public record AddClientScopeMapperCommand(string ClientScopeId, string UserPropertyName, string ClaimName) : ICommand<Result<bool>>;
}
