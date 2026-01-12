using Feijuca.Auth.Application.Requests.Realm;
using MediatR;

namespace Feijuca.Auth.Application.Queries.Realm
{
    public record ReplicateRealmCommand(ReplicateRealmRequest ReplicateRealmRequest) : IRequest<bool>;
}
