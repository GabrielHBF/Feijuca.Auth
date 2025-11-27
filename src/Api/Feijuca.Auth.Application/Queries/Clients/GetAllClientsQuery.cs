using Feijuca.Auth.Application.Responses;
using LiteBus.Queries.Abstractions;

namespace Feijuca.Auth.Application.Queries.Clients
{
    public record GetAllClientsQuery() : IQuery<IEnumerable<ClientResponse>>;
}
