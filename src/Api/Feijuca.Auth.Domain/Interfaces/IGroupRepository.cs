using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Entities;
using Feijuca.Auth.Domain.Filters;

namespace Feijuca.Auth.Domain.Interfaces
{
    public interface IGroupRepository : IBaseRepository
    {
        Task<Result<IEnumerable<Group>>> GetAllAsync(string tenant, CancellationToken cancellationToken);
        Task<Result> CreateAsync(string name, string tenant, Dictionary<string, string[]> attributes, CancellationToken cancellationToken);
        Task<Result> DeleteAsync(string id, CancellationToken cancellationToken);
        Task<Result<IEnumerable<User>>> GetUsersInGroupAsync(string id, UserFilters userFilters, int totalUsers, CancellationToken cancellationToken);
    }
}
