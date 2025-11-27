using Feijuca.Auth.Application.Mappers;
using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;
using Feijuca.Auth.Providers;
using LiteBus.Commands.Abstractions;

namespace Feijuca.Auth.Application.Commands.Client
{
    public class AddClientCommandHandler(IClientRepository clientRepository, ITenantProvider tenantService) : IRequestHandler<AddClientCommand, Result<bool>>
    {
        public async Task<Result<bool>> HandleAsync(AddClientCommand request, CancellationToken cancellationToken)
        {
            var client = request.AddClientRequest.ToClientEntity();
            var result = await clientRepository.CreateClientAsync(client, tenantService.Tenant.Name, cancellationToken);

            if (result == null)
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure(ClientErrors.CreateClientError);
        }
    }
}
