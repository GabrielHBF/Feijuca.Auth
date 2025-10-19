using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using Feijuca.Auth.Services;
using MediatR;

namespace Feijuca.Auth.Application.Commands.UserAttributes
{
    public class DeleteUserAttributesCommandHandler(IUserRepository userRepository) : IRequestHandler<DeleteUserAttributesCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteUserAttributesCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.Username, cancellationToken);

            if (user?.Data == null)
            {
                return Result<bool>.Failure(UserErrors.ErrorWhileAddedUserAttribute);
            }

            foreach (var key in request.DeleteUserAttributeRequest)
            {
                user.Data.Attributes.Remove(key);
            }

            var result = await userRepository.UpdateUserAsync(user.Data.Id, user.Data, cancellationToken);

            if (result.IsSuccess)
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure(UserErrors.ErrorWhileAddedUserAttribute);
        }

    }
}
