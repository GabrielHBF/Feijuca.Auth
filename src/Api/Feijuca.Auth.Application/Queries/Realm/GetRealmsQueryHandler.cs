using Feijuca.Auth.Application.Mappers;
using Feijuca.Auth.Application.Responses;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;

namespace Feijuca.Auth.Application.Queries.Realm
{
    internal class GetRealmsQueryHandler(IRealmRepository _realmRepository) : IRequestHandler<GetRealmsQuery, IEnumerable<RealmResponse>>
    {
        public async Task<IEnumerable<RealmResponse>> Handle(GetRealmsQuery request, CancellationToken cancellationToken)
        {
            var realms = await _realmRepository.GetAllAsync(cancellationToken);
            return realms.ToResponse();
        }
    }
}
