namespace Feijuca.Auth.Application.Requests.User
{
    public record ResetPasswordRequest(Guid Id, string NewPassword);
}
