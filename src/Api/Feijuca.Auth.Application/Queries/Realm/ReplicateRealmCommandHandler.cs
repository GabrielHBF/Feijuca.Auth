using Feijuca.Auth.Domain.Interfaces;
using MediatR;

namespace Feijuca.Auth.Application.Queries.Realm
{
    public class ReplicateRealmCommandHandler(
        IClientRepository clientRepository,
        IClientScopesRepository clientScopesRepository) : IRequestHandler<ReplicateRealmCommand, bool>
    {
        public async Task<bool> Handle(ReplicateRealmCommand request, CancellationToken cancellationToken)
        {
            if (request.ReplicateRealmRequest?.Clients?.Any() ?? false)
            {
                var clients = await clientRepository.GetClientsAsync(cancellationToken);
                foreach (var client in (clients?.Data ?? []).Where(client => request.ReplicateRealmRequest.Clients.Contains(client.ClientId)))
                {
                    await clientRepository.CreateClientAsync(client, request.ReplicateRealmRequest.Target, cancellationToken);
                }
            }

            if (request.ReplicateRealmRequest?.ClientScopes?.Any() ?? false)
            {
                var clientScopes = await clientScopesRepository.GetClientScopesAsync(cancellationToken);
                foreach (var clientScope in (clientScopes ?? []).Where(clientScope => request.ReplicateRealmRequest.Clients.Contains(clientScope.Name)))
                {
                    await clientScopesRepository.AddClientScopesAsync(clientScope, request.ReplicateRealmRequest.Target, cancellationToken);
                }
            }

            throw new NotImplementedException();
        }
    }
}
