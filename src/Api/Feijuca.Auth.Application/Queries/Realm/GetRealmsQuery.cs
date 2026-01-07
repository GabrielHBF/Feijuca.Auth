using Feijuca.Auth.Application.Responses;
using MediatR;

namespace Feijuca.Auth.Application.Queries.Realm
{
    public record GetRealmsQuery() : IRequest<IEnumerable<RealmResponse>>;
}
