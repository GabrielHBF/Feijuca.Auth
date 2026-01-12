using Feijuca.Auth.Application.Mappers;
using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;
using Feijuca.Auth.Application.Responses;
using Feijuca.Auth.Providers;

namespace Feijuca.Auth.Application.Queries.Groups
{
    public class GetAllGroupsQueryHandler(IGroupRepository groupRepository, ITenantProvider tenantProvider) : IRequestHandler<GetAllGroupsQuery, Result<IEnumerable<GroupResponse>>>
    {
        private readonly IGroupRepository _groupRepository = groupRepository;

        public async Task<Result<IEnumerable<GroupResponse>>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
        {
            var result = await _groupRepository.GetAllAsync(tenantProvider.Tenant.Name, cancellationToken);

            if (result.IsSuccess)
            {
                if (request.NotDisplayInternalGroups)
                {
                    var results = result.Data.Where(x => x.Name != "feijuca-auth-api");
                    return Result<IEnumerable<GroupResponse>>.Success(results.ToResponse());
                }

                return Result<IEnumerable<GroupResponse>>.Success(result.Data.ToResponse());
            }

            return Result<IEnumerable<GroupResponse>>.Failure(GroupErrors.CreationGroupError);
        }
    }
}
