using Feijuca.Auth.Application.Responses;
using LiteBus.Queries.Abstractions;

namespace Feijuca.Auth.Application.Queries.ClientScopes
{
    public record GetClientScopesQuery() : IQuery<IEnumerable<ClientScopesResponse>>;
}
