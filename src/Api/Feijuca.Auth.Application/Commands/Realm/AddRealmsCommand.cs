using Feijuca.Auth.Application.Requests.Realm;
using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.Realm
{
    public record AddRealmsCommand(IEnumerable<AddRealmRequest> AddRealmsRequest) : ICommand<Result<bool>>;
}
