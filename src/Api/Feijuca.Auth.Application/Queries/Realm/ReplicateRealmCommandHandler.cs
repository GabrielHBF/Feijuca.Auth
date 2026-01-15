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
                var groups = await groupRepository.GetGroupByNameAsync(targetTenant, Constants.AdminGroupName, cancellationToken);
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

                var creationUserResult = await userRepository.CreateAsync(user, cancellationToken);

                if (creationUserResult.IsSuccess)
                {
                    var keycloakUser = await userRepository.GetAsync(user.Username, targetTenant, cancellationToken);
                    await userRepository.ResetPasswordAsync(keycloakUser.Data.Id, user.Password, targetTenant, cancellationToken);
                }

                user = (await userRepository.GetAsync(user.Username, targetTenant, cancellationToken)).Data;

                await groupUsersRepository.AddUserToGroupAsync(user.Id, targetTenant, Guid.Parse(adminGroupId), cancellationToken);
            }

            if (request.ReplicateRealmRequest.ReplicationConfigurationRequest.IncludeClients)
            {
                var originClients = await clientRepository.GetClientsAsync(originTenant, cancellationToken);
                foreach (var client in originClients?.Data ?? [])
                {
                    await clientRepository.CreateClientAsync(client, targetTenant, cancellationToken);
                    var getClientJustCreated = await clientRepository.GetClientAsync(client.ClientId, targetTenant, cancellationToken);

                    if (request.ReplicateRealmRequest?.ReplicationConfigurationRequest.IncludeClientRoles ?? false)
                    {
                        var originClientRoles = await clientRoleRepository.GetRolesForClientAsync(client.Id, originTenant, cancellationToken);

                        foreach (var clientRole in originClientRoles.Data)
                        {
                            await clientRoleRepository.AddClientRoleAsync(getClientJustCreated.Data.Id,
                                clientRole.Name,
                                clientRole?.Description ?? "",
                                targetTenant,
                                cancellationToken);
                        }

                        if (request.ReplicateRealmRequest?.ReplicationConfigurationRequest.CreateAdminGroupWithAllRulesAssociated ?? false)
                        {
                            var targetClientRulesAdded = await clientRoleRepository.GetRolesForClientAsync(getClientJustCreated.Data.Id, targetTenant, cancellationToken);

                            foreach (var targetClientRuleAdded in targetClientRulesAdded.Data)
                            {
                                await groupRolesRepository.AddClientRoleToGroupAsync(adminGroupId,
                                    getClientJustCreated.Data.Id,
                                    targetClientRuleAdded.Id,
                                    targetClientRuleAdded.Name,
                                    targetTenant,
                                    cancellationToken);
                            }
                        }
                    }
                }
            }

            if (request.ReplicateRealmRequest?.ReplicationConfigurationRequest.IncludeClientScopes ?? false)
            {
                var originClientScopes = await clientScopesRepository.GetClientScopesAsync(originTenant, cancellationToken);

                foreach (var clientScope in originClientScopes ?? [])
                {
                    var result = await clientScopesRepository.AddClientScopesAsync(clientScope, targetTenant, cancellationToken);

                    if (result == false)
                    {
                        return Result<bool>.Failure(RealmErrors.ReplicateRealmError);
                    }

                    if (clientScope.Name == Constants.FeijucaApiClientName)
                    {
                        var targetClientScopes = await clientScopesRepository.GetClientScopesAsync(targetTenant, cancellationToken);
                        var clientScopeFeijuca = targetClientScopes.FirstOrDefault(x => x.Name == Constants.FeijucaApiClientName)!;
                        await clientScopesRepository.AddAudienceMapperAsync(clientScopeFeijuca.Id!, targetTenant, cancellationToken);
                    }
                }

                var clientScopeProfile = await clientScopesRepository.GetClientScopeProfileAsync(targetTenant, cancellationToken);
                await clientScopesRepository.AddUserPropertyMapperAsync(clientScopeProfile.Id!, "tenant", "tenant", targetTenant, cancellationToken);
            }

            return Result<bool>.Success(true);
        }
    }
}
