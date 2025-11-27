using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;
using LiteBus.Commands.Abstractions;
using Feijuca.Auth.Providers;

namespace Feijuca.Auth.Application.Commands.GroupRoles;

public class RemoveRoleFromGroupCommandHandler(IGroupRepository groupRepository, 
    IGroupRolesRepository roleGroupRepository, 
    IClientRoleRepository roleRepository,
    ITenantProvider tenantProvider) : IRequestHandler<RemoveRoleFromGroupCommand, Result<bool>>
{
    private readonly IGroupRepository _groupRepository = groupRepository;
    private readonly IGroupRolesRepository _roleGroupRepository = roleGroupRepository;
    private readonly IClientRoleRepository _roleRepository = roleRepository;

    public async Task<Result<bool>> HandleAsync(RemoveRoleFromGroupCommand command, CancellationToken cancellationToken)
    {
        var groupsResult = await _groupRepository.GetAllAsync(tenantProvider.Tenant.Name, cancellationToken);
        if (groupsResult.IsSuccess && groupsResult.Data.Any(x => x.Id == command.GroupId))
        {
            var rolesResult = await _roleRepository.GetRolesForClientAsync(command.RemoveRoleFromGroupRequest.ClientId, tenantProvider.Tenant.Name, cancellationToken);
            var existingRule = rolesResult.Data.FirstOrDefault(x => x.Id == command.RemoveRoleFromGroupRequest.RoleId);
            if (rolesResult.IsSuccess && existingRule != null)
            {
                await _roleGroupRepository.RemoveRoleFromGroupAsync(
                    command.RemoveRoleFromGroupRequest.ClientId,
                    command.GroupId,
                    existingRule.Id,
                    existingRule.Name,
                    cancellationToken);

                return Result<bool>.Success(true);
            }
        }

        return Result<bool>.Failure(GroupRolesErrors.RemovingRoleFromGroupError);
    }
}
