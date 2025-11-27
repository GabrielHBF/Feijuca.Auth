using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using LiteBus.Commands.Abstractions;
using Feijuca.Auth.Providers;

namespace Feijuca.Auth.Application.Commands.UserAttributes
{
    public class AddUserAttributeCommandHandler(IUserRepository userRepository) : ICommandHandler<AddUserAttributeCommand, Result<bool>>
    {
        public async Task<Result<bool>> HandleAsync(AddUserAttributeCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserName, cancellationToken);
            if (user.IsSuccess)
            {
                var userNewAttributes = user.Data.Attributes;
                foreach (var item in request.AddUserAttributesRequest.Attributes)
                {
                    userNewAttributes.TryAdd(item.Key, item.Value);
                }

                user.Data.Attributes = userNewAttributes;

                var result = await userRepository.UpdateUserAsync(user.Data.Id, user.Data, cancellationToken);

                if (result.IsSuccess)
                {
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure(UserErrors.ErrorWhileAddedUserAttribute);
            }

            return Result<bool>.Failure(UserErrors.GetUserError);
        }
    }
}
