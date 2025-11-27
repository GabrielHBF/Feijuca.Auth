using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using LiteBus.Commands.Abstractions;

namespace Feijuca.Auth.Application.Commands.Group
{
    public class DeleteGroupCommandHandler(IGroupRepository groupRepository) : ICommandHandler<DeleteGroupCommand, Result<bool>>
    {
        private readonly IGroupRepository _groupRepository = groupRepository;

        public async Task<Result<bool>> HandleAsync(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var result = await _groupRepository.DeleteAsync(request.Id, cancellationToken);

            if (result.IsSuccess)
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure(GroupErrors.DeletionGroupError);
        }
    }
}
