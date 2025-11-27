using Feijuca.Auth.Application.Commands.User;
using Microsoft.Extensions.DependencyInjection;
using Feijuca.Auth.Application.Queries.Users;
using LiteBus.Commands.Extensions.MicrosoftDependencyInjection;
using LiteBus.Queries.Extensions.MicrosoftDependencyInjection;
using LiteBus.Messaging.Extensions.MicrosoftDependencyInjection;

namespace Feijuca.Auth.Infra.CrossCutting.Extensions
{
    public static class MediatRExtension
    {
        public static IServiceCollection ConfigureLiteBus(this IServiceCollection services)
        {
            services.AddLiteBus(liteBus =>
            {
                liteBus.AddCommandModule(module =>
                {
                    module.RegisterFromAssembly(typeof(AddUserCommandHandler).Assembly);
                });

                liteBus.AddQueryModule(module =>
                {
                    module.RegisterFromAssembly(typeof(GetUsersQueryHandler).Assembly);
                });
            });

            return services;
        }
    }
}