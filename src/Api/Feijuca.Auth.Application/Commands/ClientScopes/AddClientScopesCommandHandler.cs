using Feijuca.Auth.Application.Mappers;
using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;
using Feijuca.Auth.Providers;

namespace Feijuca.Auth.Application.Commands.ClientScopes;

public class AddClientScopesCommandHandler(IClientScopesRepository clientScopesRepository, ITenantProvider tenantService) : IRequestHandler<AddClientScopesCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AddClientScopesCommand command, CancellationToken cancellationToken)
    {
        foreach (var clientScope in command.AddClientScopesRequest)
        {
            var scopeEntity = clientScope.ToClientScopesEntity();
            var result = await clientScopesRepository.AddClientScopesAsync(scopeEntity, tenantService.Tenant.Name, cancellationToken);

            if (string.IsNullOrEmpty(result))
            {
                return Result<bool>.Failure(ClientScopesErrors.CreateClientScopesError);
            }
        }


        return Result<bool>.Success(true);
    }
}
