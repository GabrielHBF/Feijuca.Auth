namespace Feijuca.Auth.Application.Responses
{
    public record RealmResponse(string Issuer, string Realm, string DisplayName, bool Enabled);
}
