using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using LiteBus.Commands.Abstractions;
using MediatR;
using Feijuca.Auth.Providers;

namespace Feijuca.Auth.Application.Commands.ClientRole
{
    public class AddClientRoleCommandHandler(IClientRoleRepository clientRolesRepository, ITenantProvider tenantProvider) : IRequestHandler<AddClientRoleCommand, Result<bool>>
    {
        private readonly IClientRoleRepository _roleRepository = clientRolesRepository;

        public async Task<Result<bool>> HandleAsync(AddClientRoleCommand request, CancellationToken cancellationToken)
        {
            foreach (var clientRole in request.AddClientRolesRequest)
            {
                var result = await _roleRepository.AddClientRoleAsync(clientRole.ClientId, 
                    clientRole.Name, 
                    clientRole.Description, 
                    tenantProvider.Tenant.Name,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return Result<bool>.Failure(RoleErrors.AddRoleErrors);
                }
            }

            return Result<bool>.Success(true);            
        }
    }
}
