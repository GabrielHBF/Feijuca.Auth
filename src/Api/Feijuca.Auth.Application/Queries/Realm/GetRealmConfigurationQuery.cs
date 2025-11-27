using LiteBus.Queries.Abstractions;

namespace Feijuca.Auth.Application.Queries.Realm
{
    public record GetRealmConfigurationQuery(string Name) : IQuery<string>;
}
