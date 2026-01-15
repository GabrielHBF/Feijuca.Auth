namespace Feijuca.Auth.Application.Requests.Realm
{
    public record ReplicateRealmRequest(string Tenant, ReplicationConfigurationRequest ReplicationConfigurationRequest);
}
