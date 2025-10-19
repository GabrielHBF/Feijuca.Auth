using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;

namespace Feijuca.Auth.Application.Commands.User
{
    public class RevokeUserSessionsCommandHandler(IUserRepository userRepository) : IRequestHandler<RevokeUserSessionsCommand, Result>
    {
        private readonly IUserRepository _userRepository = userRepository;
        public async Task<Result> Handle(RevokeUserSessionsCommand request, CancellationToken cancellationToken)
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
