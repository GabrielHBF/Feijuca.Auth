using Feijuca.Auth.Application.Requests.UsersAttributes;
using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.UserAttributes
{
    public record AddUserAttributeCommand(string UserName, AddUserAttributesRequest AddUserAttributesRequest) : ICommand<Result<bool>>;
}
