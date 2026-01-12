using Feijuca.Auth.Common;
using Feijuca.Auth.Common.Errors;
using Feijuca.Auth.Domain.Interfaces;
using Feijuca.Auth.Providers;
using Mattioli.Configurations.Models;
using MediatR;

namespace Feijuca.Auth.Application.Queries.Realm
{
    public class ReplicateRealmCommandHandler(
        IUserRepository userRepository,
        IClientRepository clientRepository,
        IClientScopesRepository clientScopesRepository,
        IClientRoleRepository clientRoleRepository,
        IGroupRepository groupRepository,
        IGroupUsersRepository groupUsersRepository,
        IGroupRolesRepository groupRolesRepository,
        ITenantProvider tenantProvider) : IRequestHandler<ReplicateRealmCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(ReplicateRealmCommand request, CancellationToken cancellationToken)
        {
            var targetTenant = request.ReplicateRealmRequest.Tenant;
            var originTenant = tenantProvider.Tenant.Name;

            var adminGroupId = string.Empty;
            if (request.ReplicateRealmRequest!.ReplicationConfigurationRequest.CreateAdminGroupWithAllRulesAssociated)
            {
                await groupRepository.CreateAsync(Constants.AdminGroupName, targetTenant, [], cancellationToken);
                var groups = await groupRepository.GetAllAsync(targetTenant, cancellationToken);
                adminGroupId = groups.Data.First(x => x.Name == Constants.AdminGroupName).Id;
            }

            if (request.ReplicateRealmRequest!.ReplicationConfigurationRequest.AdminUser.Username != string.Empty)
            {
                var user = new Domain.Entities.User(request.ReplicateRealmRequest.ReplicationConfigurationRequest.AdminUser.Username,
                    request.ReplicateRealmRequest.ReplicationConfigurationRequest.AdminUser.Password,
                    request.ReplicateRealmRequest.ReplicationConfigurationRequest.AdminUser.Username,
                    request.ReplicateRealmRequest.ReplicationConfigurationRequest.AdminUser.Username,
                    request.ReplicateRealmRequest.ReplicationConfigurationRequest.AdminUser.Username,
                     new Dictionary<string, string[]>
                     {
                         { "Tenant", [targetTenant] }
                     });

                await userRepository.CreateAsync(user, cancellationToken);

                user = (await userRepository.GetAsync(user.Username, targetTenant, cancellationToken)).Data;

                await groupUsersRepository.AddUserToGroupAsync(user.Id, Guid.Parse(adminGroupId), cancellationToken);
            }

            if (request.ReplicateRealmRequest.ReplicationConfigurationRequest.IncludeClients)
            {

                var originClients = await clientRepository.GetClientsAsync(originTenant, cancellationToken);
                foreach (var client in originClients?.Data ?? [])
                {
                    var result = await clientRepository.CreateClientAsync(client, targetTenant, cancellationToken);
                    var targetClientCreated = await clientRepository.GetClientAsync(client.ClientId, targetTenant, cancellationToken);

                    if (request.ReplicateRealmRequest?.ReplicationConfigurationRequest.IncludeClientRoles ?? false)
                    {
                        var clientRoles = await clientRoleRepository.GetRolesForClientAsync(client.Id, originTenant, cancellationToken);

                        foreach (var clientRole in clientRoles.Data)
                        {
                            await clientRoleRepository.AddClientRoleAsync(targetClientCreated.Data.Id,
                                clientRole.Name,
                                clientRole?.Description ?? "",
                                request.ReplicateRealmRequest!.Tenant!,
                                cancellationToken);
                        }
                        
                        if (request.ReplicateRealmRequest?.ReplicationConfigurationRequest.CreateAdminGroupWithAllRulesAssociated ?? false)
                        {
                            var clientRulesAdded = await clientRoleRepository.GetRolesForClientAsync(targetClientCreated.Data.Id, targetTenant, cancellationToken);

                            foreach (var clientRuleAdded in clientRulesAdded.Data)
                            {
                                await groupRolesRepository.AddClientRoleToGroupAsync(adminGroupId,
                                    targetClientCreated.Data.Id,
                                    clientRuleAdded.Id,
                                    clientRuleAdded.Name,
                                    targetTenant,
                                    cancellationToken);
                            }
                        }
                    }
                }
            }

            if (request.ReplicateRealmRequest?.ReplicationConfigurationRequest.IncludeClientScopes ?? false)
            {
                var clientScopes = await clientScopesRepository.GetClientScopesAsync(cancellationToken);
                foreach (var clientScope in clientScopes ?? [])
                {
                    var result = await clientScopesRepository.AddClientScopesAsync(clientScope, targetTenant, cancellationToken);

                    if (result == false)
                    {
                        return Result<bool>.Failure(RealmErrors.ReplicateRealmError);
                    }
                }
            }

            return Result<bool>.Success(true);
        }
    }
}
