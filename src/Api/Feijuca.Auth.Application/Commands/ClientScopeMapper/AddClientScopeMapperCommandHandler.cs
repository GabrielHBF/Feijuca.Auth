using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;
using Feijuca.Auth.Providers;

namespace Feijuca.Auth.Application.Commands.ClientScopeMapper
{
    public class AddClientScopeMapperCommandHandler(IClientScopesRepository clientScopesRepository, ITenantProvider tenantProvider) : IRequestHandler<AddClientScopeMapperCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(AddClientScopeMapperCommand request, CancellationToken cancellationToken)
        {
            var result = await clientScopesRepository.AddUserPropertyMapperAsync(request.ClientScopeId, 
                request.UserPropertyName, 
                request.ClaimName,
                tenantProvider.Tenant.Name,
                cancellationToken);

            if (result)
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure(ClientScopesErrors.CreateAudienceMapperProtocolError);
        }
    }
}
