namespace Feijuca.Auth.Application.Requests.GroupRoles;

public record RemoveClientRoleToGroupRequest(string ClientId, Guid RoleId);