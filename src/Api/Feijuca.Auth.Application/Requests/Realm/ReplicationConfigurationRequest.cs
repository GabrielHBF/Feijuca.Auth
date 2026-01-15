using Feijuca.Auth.Application.Requests.Auth;

namespace Feijuca.Auth.Application.Requests.Realm
{
    public record ReplicationConfigurationRequest(bool IncludeClients, 
        bool IncludeClientRoles, 
        bool IncludeClientScopes,
        bool CreateAdminGroupWithAllRulesAssociated,
        LoginUserRequest AdminUser);
}
