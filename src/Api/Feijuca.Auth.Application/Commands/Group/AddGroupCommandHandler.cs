using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;
using Feijuca.Auth.Providers;

namespace Feijuca.Auth.Application.Commands.Group
{
    public class AddGroupCommandHandler(IGroupRepository groupRepository, ITenantProvider tenantProvider) : IRequestHandler<AddGroupCommand, Result<bool>>
    {
        private readonly IGroupRepository _groupRepository = groupRepository;

        public async Task<Result<bool>> Handle(AddGroupCommand request, CancellationToken cancellationToken)
        {
            var result = await _groupRepository.CreateAsync(request.AddGroupRequest.Name,
                tenantProvider.Tenant.Name,
                request.AddGroupRequest.Attributes, 
                cancellationToken);

            if (result.IsSuccess)
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure(result.Error);
        }
    }
}
