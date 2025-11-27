using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;
using LiteBus.Commands.Abstractions;

namespace Feijuca.Auth.Application.Commands.GroupUser
{
    public class RemoveUserFromGroupCommandHandler(IGroupUsersRepository userGroupRepository) : ICommandHandler<RemoveUserFromGroupCommand, Result<bool>>
    {
        private readonly IGroupUsersRepository _userGroupRepository = userGroupRepository;

        public async Task<Result<bool>> HandleAsync(RemoveUserFromGroupCommand request, CancellationToken cancellationToken)
        {
            var result = await _userGroupRepository.RemoveUserFromGroupAsync(request.UserId, request.GroupId, cancellationToken);

            if (result.IsSuccess)
            {
                return Result<bool>.Success(result.Data);
            }

            return Result<bool>.Failure(result.Error);
        }
    }
}
