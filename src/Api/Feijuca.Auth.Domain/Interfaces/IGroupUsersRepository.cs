using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Domain.Interfaces
{
    public interface IGroupUsersRepository : IBaseRepository
    {
        Task<Result<bool>> AddUserToGroupAsync(Guid userId, string tenant, Guid groupId, CancellationToken cancellationToken);
        Task<Result<bool>> RemoveUserFromGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);
    }
}
