using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Errors;

public static class FeijucaErrors
{
    public static readonly Error GetUserErrors = new("ErrorGetUsers", "An error occured while tried get users.");
    public static readonly Error GetGroupErrors = new("ErrorGetGroups", "An error occured while tried get groups.");
    public static readonly Error GetRealmErrors = new("ErrorGetRealms", "An error occured while tried get realms.");
    public static readonly Error GenerateTokenError = new("GenerateTokenError", "An error occured while generate JWT Token.");
}

