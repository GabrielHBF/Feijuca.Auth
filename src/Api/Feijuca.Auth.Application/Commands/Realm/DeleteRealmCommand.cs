using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.Realm
{
    public record DeleteRealmCommand(string RealmName) : ICommand<Result<bool>>;
}
