using Mattioli.Configurations.Models;
using Feijuca.Auth.Application.Responses;
using LiteBus.Queries.Abstractions;

namespace Feijuca.Auth.Application.Queries.Groups
{
    public record GetAllGroupsQuery(bool NotDisplayInternalGroups) : IQuery<Result<IEnumerable<GroupResponse>>>;
}
