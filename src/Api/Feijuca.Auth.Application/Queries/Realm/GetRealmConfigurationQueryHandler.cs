using Feijuca.Auth.Domain.Interfaces;
using LiteBus.Queries.Abstractions;
using MediatR;

namespace Feijuca.Auth.Application.Queries.Realm
{
    public class GetRealmConfigurationQueryHandler(IRealmRepository _realmRepository) : IQueryHandler<GetRealmConfigurationQuery, string>
    {
        public async Task<string> HandleAsync(GetRealmConfigurationQuery request, CancellationToken cancellationToken)
        {
            await _realmRepository.GetRealmConfigAsync(request.Name, cancellationToken);
            throw new NotImplementedException();
        }
    }
}
