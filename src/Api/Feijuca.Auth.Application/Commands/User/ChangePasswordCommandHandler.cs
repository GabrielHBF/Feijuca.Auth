using Feijuca.Auth.Common.Errors;
using Feijuca.Auth.Domain.Interfaces;
using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Commands.User
{
    public class ChangePasswordCommandHandler(IUserRepository userRepository) : ICommandHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        public async Task<Result<bool>> HandleAsync(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.ResetPasswordAsync(request.ChangePasswordRequest.Id,request.ChangePasswordRequest.NewPassword, request.Tenant, cancellationToken);

            if (result.IsSuccess)
            {
                return Result<bool>.Success(true);
            }
            return Result<bool>.Failure(UserErrors.ChangePasswordError);
        }
    }
}