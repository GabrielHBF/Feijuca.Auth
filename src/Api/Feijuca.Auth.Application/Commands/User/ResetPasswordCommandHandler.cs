using Feijuca.Auth.Common.Errors;
using Feijuca.Auth.Domain.Interfaces;
using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.User
{
    public class ResetPasswordCommandHandler(IUserRepository userRepository) : ICommandHandler<ResetPasswordCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        public async Task<Result<bool>> HandleAsync(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.ResetPasswordAsync(request.ChangePasswordRequest.Id,request.ChangePasswordRequest.NewPassword, cancellationToken);

            if (result.IsSuccess)
            {
                return Result<bool>.Success(true);
            }
            return Result<bool>.Failure(UserErrors.ChangePasswordError);
        }
    }
}