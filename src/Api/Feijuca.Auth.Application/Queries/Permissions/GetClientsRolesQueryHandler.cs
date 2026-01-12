using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;
using Feijuca.Auth.Application.Responses;
using Feijuca.Auth.Providers;

namespace Feijuca.Auth.Application.Queries.Permissions
{
    public class GetClientsRolesQueryHandler(IClientRepository clientRepository, 
        IClientRoleRepository roleRepository,
        ITenantProvider tenantProvider) : IRequestHandler<GetClientRolesQuery, Result<IEnumerable<ClientRoleResponse>>>
    {
        private readonly IClientRepository _clientRepository = clientRepository;
        private readonly IClientRoleRepository _roleRepository = roleRepository;

        public async Task<Result<IEnumerable<ClientRoleResponse>>> Handle(GetClientRolesQuery request, CancellationToken cancellationToken)
        {
            var result = await _clientRepository.GetClientsAsync(tenantProvider.Tenant.Name, cancellationToken);
            if (result.IsSuccess)
            {
                var roleResponse = new List<ClientRoleResponse>();
                foreach (var client in result.Data)
                {
                    var rolesResult = await _roleRepository.GetRolesForClientAsync(client.Id, tenantProvider.Tenant.Name, cancellationToken);

                    if (rolesResult.IsSuccess)
                    {
                        var rolesResponse = rolesResult.Data.Select(x => new RoleResponse(x.Id, x.Name, x.Description ?? "", false, false, string.Empty));
                        roleResponse.Add(new ClientRoleResponse(client.ClientId, client.Id, rolesResponse));
                    }
                }

                return Result<IEnumerable<ClientRoleResponse>>.Success(roleResponse);
            }

            return Result<IEnumerable<ClientRoleResponse>>.Failure(RoleErrors.GetRoleErrors);
        }
    }
}
