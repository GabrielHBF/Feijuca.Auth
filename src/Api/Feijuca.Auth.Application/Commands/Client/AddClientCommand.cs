using Feijuca.Auth.Application.Requests.Client;
using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.Client
{
    public record AddClientCommand(AddClientRequest AddClientRequest) : ICommand<Result<bool>>;
}
