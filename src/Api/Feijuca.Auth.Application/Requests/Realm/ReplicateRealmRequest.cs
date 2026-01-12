namespace Feijuca.Auth.Application.Requests.Realm
{
    public record ReplicateRealmRequest(string Target,
        IEnumerable<string> Clients,
        IEnumerable<string> ClientScopes);
}
