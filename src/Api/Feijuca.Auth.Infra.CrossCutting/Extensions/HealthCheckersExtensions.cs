using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Feijuca.Auth.Infra.CrossCutting.Extensions
{
    public static class HealthCheckersExtensions
    {
        public static IServiceCollection AddHealthCheckers(this IServiceCollection services)
        {
            services
              .AddHealthChecks()
              .AddMongoDb(
                clientFactory: sp => sp.GetRequiredService<IMongoClient>(),
                name: "MongoDB",
                tags: ["db", "mongo"]);

            return services;
        }

        public static void UseHealthCheckers(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }
    }
}
