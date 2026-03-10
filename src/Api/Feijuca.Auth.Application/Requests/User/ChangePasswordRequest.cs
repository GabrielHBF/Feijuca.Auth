namespace Feijuca.Auth.Application.Requests.User
{
    public record ChangePasswordRequest(Guid Id, string NewPassword);
}
