using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using LiteBus.Commands.Abstractions;

namespace Feijuca.Auth.Application.Commands.User
{
    public class RevokeUserSessionsCommandHandler(IUserRepository userRepository) : ICommandHandler<RevokeUserSessionsCommand, Result>
    {
        private readonly IUserRepository _userRepository = userRepository;
        public async Task<Result> HandleAsync(RevokeUserSessionsCommand request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.RevokeSessionsByUserIdAsync(request.UserId, cancellationToken);

            if (result.IsSuccess)
            {
                return result;
            }

            return Result.Failure(result.Error);
        }
    }
}
