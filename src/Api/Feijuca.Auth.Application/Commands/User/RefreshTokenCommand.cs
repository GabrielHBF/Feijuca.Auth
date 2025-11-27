using Feijuca.Auth.Http.Responses;
using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.User
{
    public record RefreshTokenCommand(string Tenant, string RefreshToken) : ICommand<Result<TokenDetailsResponse>>;
}
