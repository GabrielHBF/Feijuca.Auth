using Feijuca.Auth.Application.Mappers;
using Feijuca.Auth.Application.Responses;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;

namespace Feijuca.Auth.Application.Queries.Realm
{
    public class GetRealmsQueryHandler(IRealmRepository _realmRepository, IConfigRepository configRepository) : IRequestHandler<GetRealmsQuery, IEnumerable<RealmResponse>>
    {
        public async Task<IEnumerable<RealmResponse>> Handle(GetRealmsQuery request, CancellationToken cancellationToken)
        {
            var config = await configRepository.GetConfigAsync();
            var realms = await _realmRepository.GetAllAsync(cancellationToken);
            return realms.ToResponse(config.ServerSettings.Url);
        }
    }
}
