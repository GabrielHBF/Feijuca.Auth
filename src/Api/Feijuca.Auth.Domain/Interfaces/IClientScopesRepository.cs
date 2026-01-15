using Feijuca.Auth.Domain.Entities;

namespace Feijuca.Auth.Domain.Interfaces
{
    public interface IClientScopesRepository : IBaseRepository
    {
        Task<bool> AddClientScopesAsync(ClientScopeEntity clientScopesEntity, string tenant, CancellationToken cancellationToken);
        Task<bool> AddUserPropertyMapperAsync(string clientScopeId,
            string userPropertyName,
            string claimName,
            string tenant,
            CancellationToken cancellationToken);

        Task<bool> AddClientScopeToClientAsync(string clientId, string clientScopeId, bool isOptional, CancellationToken cancellationToken);
        Task<IEnumerable<ClientScopeEntity>> GetClientScopesAsync(string tenant, CancellationToken cancellationToken);
        Task<bool> AddAudienceMapperAsync(string clientScopeId, string tenant, CancellationToken cancellationToken);
        Task<ClientScopeEntity> GetClientScopeProfileAsync(string tenant, CancellationToken cancellationToken);
    }
}
