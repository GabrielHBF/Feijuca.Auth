using Feijuca.Auth.Application.Mappers;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;
using Feijuca.Auth.Application.Responses;
using Feijuca.Auth.Providers;

namespace Feijuca.Auth.Application.Queries.ClientScopes;

public class GetClientScopesQueryHandler(IClientScopesRepository clientScopesRepository, ITenantProvider tenantProvider) 
    : IRequestHandler<GetClientScopesQuery, IEnumerable<ClientScopesResponse>>
{
    public async Task<IEnumerable<ClientScopesResponse>> Handle(GetClientScopesQuery request, CancellationToken cancellationToken)
    {
        var scopes = await clientScopesRepository.GetClientScopesAsync(tenantProvider.Tenant.Name, cancellationToken);
        return scopes.ToClientScopesResponse();
    }
}
