using Feijuca.Auth.Application.Requests.UsersAttributes;
using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.UserAttributes
{
    public record UpdateUserAttributesCommand(string Username, UserAttributeRequest UpdateUserAttributeRequest) : ICommand<Result<bool>>;
}
