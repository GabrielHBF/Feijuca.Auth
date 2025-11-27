using Feijuca.Auth.Application.Mappers;
using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using MediatR;
using LiteBus.Commands.Abstractions;

namespace Feijuca.Auth.Application.Commands.Realm
{
    public class AddRealmsCommandHandler(IRealmService realmService) : ICommandHandler<AddRealmsCommand, Result<bool>>
    {
        public async Task<Result<bool>> HandleAsync(AddRealmsCommand request, CancellationToken cancellationToken)
        {
            foreach (var realmRequest in request.AddRealmsRequest)
            {
                var result = await realmService.AddNewRealmAsync(realmRequest.ToRealmEntity(), cancellationToken);
                if (!result.IsSuccess)
                {
                    return Result<bool>.Failure(RealmErrors.CreateRealmError);
                }
            }

            return Result<bool>.Success(true);
        }
    }
}
