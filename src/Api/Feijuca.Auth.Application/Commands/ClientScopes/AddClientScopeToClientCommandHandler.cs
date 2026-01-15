using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;
using Feijuca.Auth.Providers;

namespace Feijuca.Auth.Application.Commands.ClientScopes
{
    public class AddClientScopeToClientCommandHandler(IClientScopesRepository clientScopesRepository, ITenantProvider tenantProvider) : IRequestHandler<AddClientScopeToClientCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(AddClientScopeToClientCommand request, CancellationToken cancellationToken)
        {
            var result = await clientScopesRepository.AddClientScopeToClientAsync(
                request.AddClientScopeToClientRequest.ClientId,
                tenantProvider.Tenant.Name,
                request.AddClientScopeToClientRequest.ClientScopeId,
                request.AddClientScopeToClientRequest.IsOpticionalScope, 
                cancellationToken);

            if(result)
                return Result<bool>.Success(true);

            return Result<bool>.Failure(ClientErrors.AddClientRoleError);
        }
    }
}
