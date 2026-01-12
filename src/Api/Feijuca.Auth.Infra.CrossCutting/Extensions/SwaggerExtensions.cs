using Feijuca.Auth.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace Feijuca.Auth.Infra.CrossCutting.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwagger(
            this IServiceCollection services,
            KeycloakSettings? keycloakSettings)
        {
            services.AddSwaggerGen(options =>
            {
                var realmName = keycloakSettings?
                    .Realms?
                    .FirstOrDefault(x => x.DefaultSwaggerTokenGeneration)?
                    .Name;

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Feijuca.Auth.Api",
                    Version = "v1"
                });


                if (!string.IsNullOrEmpty(realmName))
                {
                    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "JWT Authorization header using the Bearer scheme."
                    });

                    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("bearer", document)] = []
                    });
                }

                options.IncludeXmlComments(
                    Path.Combine(AppContext.BaseDirectory, "Feijuca.Auth.Api.xml"),
                    includeControllerXmlComments: true
                );
            });

            return services;
        }
    }
}
