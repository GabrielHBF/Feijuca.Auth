using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;
using Feijuca.Auth.Providers;

namespace Feijuca.Auth.Application.Commands.ClientScopeProtocol
{
    public class AddClientScopeAudienceProtocolMapperCommandHandler(IClientScopesRepository clientScopesRepository, ITenantProvider tenantProvider) : IRequestHandler<AddClientScopeAudienceProtocolMapperCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(AddClientScopeAudienceProtocolMapperCommand request, CancellationToken cancellationToken)
        {
            var result = await clientScopesRepository.AddAudienceMapperAsync(request.ClientScopeId, tenantProvider.Tenant.Name, cancellationToken);

            if (result)
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure(ClientScopesErrors.CreateAudienceMapperProtocolError);
        }
    }
}
