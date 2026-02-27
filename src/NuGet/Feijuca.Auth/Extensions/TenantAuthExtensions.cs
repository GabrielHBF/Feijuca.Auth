using Feijuca.Auth.Models;
using Feijuca.Auth.Providers;
using Feijuca.Auth.Validators;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace Feijuca.Auth.Extensions;

public static class TenantAuthExtensions
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IEnumerable<Realm>? realms = null)
    {
        services.AddHttpContextAccessor();
        services.AddKeyCloakAuth(realms);

        return services;
    }

    public static IServiceCollection AddKeyCloakAuth(this IServiceCollection services, IEnumerable<Realm>? realms = null)
    {
        services
            .AddSingleton<JwtSecurityTokenHandler>()
            .AddScoped<ITenantProvider, TenanatProvider>()
            .AddSingleton<IOpenIdConfigurationProvider, OpenIdConfigurationProvider>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddKeycloakWebApi(
                options =>
                {
                    options.Resource = "feijuca-auth-api";
                },
                options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = OnMessageReceived(services, realms),
                        OnAuthenticationFailed = OnAuthenticationFailed,
                        OnChallenge = OnChallenge
                    };
                });

        ConfigureAuthorization(services, []);

        return services;
    }

    private static Func<MessageReceivedContext, Task> OnMessageReceived(IServiceCollection services, IEnumerable<Realm>? realms = null)
    {
        return async context =>
        {
            var endpoint = context.HttpContext.GetEndpoint();
            var hasAuthorize = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null;
            if (!hasAuthorize)
            {
                context.NoResult();
                return;
            }

            var tokenJwt = context.Request.Headers.Authorization.FirstOrDefault() ?? context.Request.Query["access_token"].FirstOrDefault();

            if (string.IsNullOrEmpty(tokenJwt))
            {
                context.HttpContext.Items["AuthError"] = "Invalid JWT token!";
                context.HttpContext.Items["AuthStatusCode"] = 401;
                context.Fail("Invalid JWT token!");
                return;
            }

            var token = tokenJwt.Replace("Bearer ", "");

            var openIdProvider = context.HttpContext.RequestServices.GetRequiredService<IOpenIdConfigurationProvider>();

            await TokenValidator.ProcessTokenValidationAsync(context, token, openIdProvider);
        };
    }

    private static Task OnAuthenticationFailed(AuthenticationFailedContext context)
    {
        var errorMessage = context.HttpContext.Items["AuthError"] as string ?? "Authentication failed!";
        var statusCode = context.HttpContext.Items["AuthStatusCode"] as int? ?? 401;
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(new { error = errorMessage });
    }

    private static async Task OnChallenge(JwtBearerChallengeContext context)
    {
        if (!context.Response.HasStarted)
        {
            var errorMessage = context.HttpContext.Items["AuthError"] as string ?? "Authentication failed!";
            var statusCode = context.HttpContext.Items["AuthStatusCode"] as int? ?? 401;
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { Message = errorMessage });
        }

        context.HandleResponse();
    }

    private static void ConfigureAuthorization(IServiceCollection services, IEnumerable<Policy>? policySettings)
    {
        services
           .AddAuthorization()
           .AddKeycloakAuthorization();

        foreach (var policy in (policySettings ?? []).Where(policy => !string.IsNullOrEmpty(policy.Name)))
        {
            services
                .AddAuthorizationBuilder()
                .AddPolicy(policy.Name, p =>
                {
                    p.RequireResourceRolesForClient(
                        "feijuca-auth-api",
                        [.. policy.Roles!]);
                });
        }
    }
}