using Feijuca.Auth.Application.Requests.Realm;
using Mattioli.Configurations.Models;
using MediatR;

namespace Feijuca.Auth.Application.Queries.Realm
{
    public record ReplicateRealmCommand(ReplicateRealmRequest ReplicateRealmRequest) : IRequest<Result<bool>>;
}
