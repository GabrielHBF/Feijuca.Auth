using Feijuca.Auth.Application.Mappers;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using Feijuca.Auth.Http.Responses;
using MediatR;

namespace Feijuca.Auth.Application.Commands.User
{
    public class RefreshTokenCommandHandler(IUserRepository userRepository) : IRequestHandler<RefreshTokenCommand, Result<TokenDetailsResponse>>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<TokenDetailsResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var tokenDetails = await _userRepository.RefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (tokenDetails.IsSuccess)
            {
                return Result<TokenDetailsResponse>.Success(tokenDetails.Data.ToTokenDetailResponse());
            }

            return Result<TokenDetailsResponse>.Failure(tokenDetails.Error);
        }
    }
}
