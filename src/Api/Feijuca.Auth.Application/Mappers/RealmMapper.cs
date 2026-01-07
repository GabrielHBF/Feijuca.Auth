using Feijuca.Auth.Application.Requests.Pagination;
using Feijuca.Auth.Application.Requests.Realm;
using Feijuca.Auth.Application.Requests.User;
using Feijuca.Auth.Application.Responses;
using Feijuca.Auth.Domain.Entities;
using Feijuca.Auth.Domain.Filters;
using Feijuca.Auth.Models;

namespace Feijuca.Auth.Application.Mappers
{
    public static class RealmMapper
    {
        public static RealmEntity ToRealmEntity(this AddRealmRequest addRealmRequest)
        {
            return new RealmEntity
            {
                Realm = addRealmRequest.Name,
                DisplayName = addRealmRequest.Description,
                Enabled = true,
                DefaultSwaggerTokenGeneration = addRealmRequest.DefaultSwaggerTokenGeneration
            };
        }

        public static IEnumerable<RealmResponse> ToResponse(this IEnumerable<RealmEntity> results)
        {
            return results.Select(r => new RealmResponse(
                r.Realm,
                r.DisplayName ?? string.Empty,
                r.Enabled
            ));
        }
    }
}
