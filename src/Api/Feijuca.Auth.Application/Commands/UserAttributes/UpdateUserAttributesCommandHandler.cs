using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using Feijuca.Auth.Services;

using MediatR;

namespace Feijuca.Auth.Application.Commands.UserAttributes
{
    public class UpdateUserAttributesCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserAttributesCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateUserAttributesCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.Username, cancellationToken);

            var oldAttributesUpdated = user.Data.Attributes;

            foreach (var newAttribute in request.UpdateUserAttributeRequest.Attributes)
            {
                if (oldAttributesUpdated.TryGetValue(newAttribute.Key, out string[]? _))
                {
                    oldAttributesUpdated[newAttribute.Key] = newAttribute.Value;
                }
            }

            user.Data.Attributes = oldAttributesUpdated;

            var result = await userRepository.UpdateUserAsync(user.Data.Id, user.Data, cancellationToken);

            if (result.IsSuccess)
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure(UserErrors.ErrorWhileAddedUserAttribute);
        }
    }
}
