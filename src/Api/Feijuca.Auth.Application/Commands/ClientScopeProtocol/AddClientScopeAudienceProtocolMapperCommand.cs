using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.ClientScopeProtocol
{
    public record AddClientScopeAudienceProtocolMapperCommand(string ClientScopeId) : ICommand<Result<bool>>;
}
