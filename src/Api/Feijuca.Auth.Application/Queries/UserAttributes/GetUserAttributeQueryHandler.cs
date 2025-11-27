using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using LiteBus.Queries.Abstractions;

namespace Feijuca.Auth.Application.Queries.UserAttributes
{
    public class GetUserAttributeQueryHandler(IUserRepository userRepository) : IQueryHandler<GetUserAttributeQuery, Result<Dictionary<string, string[]>>>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<Dictionary<string, string[]>>> HandleAsync(GetUserAttributeQuery request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetAsync(request.Username, cancellationToken);

            if (!result.IsSuccess)
            {
                return Result<Dictionary<string, string[]>>.Failure(UserErrors.GetAllUsersError);
            }

            var attributes = result.Data.Attributes;
            return Result<Dictionary<string, string[]>>.Success(attributes);
        }


    }
}
