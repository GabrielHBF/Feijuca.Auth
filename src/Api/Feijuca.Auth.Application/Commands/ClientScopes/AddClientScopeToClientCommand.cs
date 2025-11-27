using Feijuca.Auth.Application.Requests.Client;
using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.ClientScopes
{
    public record AddClientScopeToClientCommand(AddClientScopeToClientRequest AddClientScopeToClientRequest) : ICommand<Result<bool>>;
}
