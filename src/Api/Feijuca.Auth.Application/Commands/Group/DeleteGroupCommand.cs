using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.Group
{
    public record DeleteGroupCommand(string Id) : ICommand<Result<bool>>;
}
