using Feijuca.Auth.Common.Errors;
using Feijuca.Auth.Common.Models;
using Feijuca.Auth.Domain.Interfaces;
using LiteBus.Queries.Abstractions;
using Mattioli.Configurations.Models;

namespace Feijuca.Auth.Application.Queries.Config
{
    public class GetConfigQueryHandler(IConfigRepository configRepository) : IQueryHandler<GetConfigQuery, Result<KeycloakSettings>>
    {
        private readonly IConfigRepository _configRepository = configRepository;

        public async Task<Result<KeycloakSettings>> HandleAsync(GetConfigQuery request, CancellationToken cancellationToken)
        {
            var result = await _configRepository.GetConfigAsync();

            if (result is null)
            {
                return Result<KeycloakSettings>.Failure(ConfigErrors.NoConfigInserted);
            }

            return Result<KeycloakSettings>.Success(result);
        }
    }
}
