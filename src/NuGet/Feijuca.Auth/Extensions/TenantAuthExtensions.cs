using Feijuca.Auth.Models;
using Feijuca.Auth.Providers;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Feijuca.Auth.Extensions;

public static class TenantAuthExtensions
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, FeijucaAuthConfiguration feijucaAuthConfiguration)
    {
        services.AddHttpContextAccessor();
        services.AddKeyCloakAuth(feijucaAuthConfiguration);

        return services;
    }

    public static IServiceCollection AddKeyCloakAuth(this IServiceCollection services, FeijucaAuthConfiguration feijucaAuthConfiguration)
    {
        var keycloakBaseUrl = feijucaAuthConfiguration.Url.TrimEnd('/');

        services
            .AddSingleton<IOidcConfigManagerCache, OidcConfigManagerCache>()
            .AddSingleton<JwtSecurityTokenHandler>()
            .AddScoped<ITenantProvider, TenanatProvider>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddKeycloakWebApi(
                options =>
                {
                    options.Resource = "feijuca-auth-api";
                },
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidAudience = "feijuca-auth-api",
                        ClockSkew = TimeSpan.FromMinutes(2),

                        IssuerValidator = (issuer, securityToken, validationParameters) =>
                        {
                            if (string.IsNullOrWhiteSpace(issuer))
                                throw new SecurityTokenInvalidIssuerException("Missing issuer");

                            if (!issuer.StartsWith(keycloakBaseUrl + "/", StringComparison.OrdinalIgnoreCase))
                                throw new SecurityTokenInvalidIssuerException("Issuer outside configured Keycloak base url");

                            if (!Uri.TryCreate(issuer, UriKind.Absolute, out var uri) ||
                                !uri.AbsolutePath.StartsWith("/realms/", StringComparison.OrdinalIgnoreCase) ||
                                string.IsNullOrEmpty(uri.AbsolutePath.Substring("/realms/".Length)))
                            {
                                throw new SecurityTokenInvalidIssuerException("Invalid issuer path");
                            }

                            if (!string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
                                throw new SecurityTokenInvalidIssuerException("Invalid issuer");

                            return issuer;
                        }
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = OnMessageReceived(),
                        OnAuthenticationFailed = OnAuthenticationFailed,
                        OnChallenge = OnChallenge
                    };
                });

        ConfigureAuthorization(services, []);

        return services;
    }

    private static Func<MessageReceivedContext, Task> OnMessageReceived()
    {
        return context =>
        {
            try
            {
                var tokenJwt =
                    context.Request.Headers.Authorization.FirstOrDefault()
                    ?? context.Request.Query["access_token"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(tokenJwt))
                {
                    context.Fail("Missing token");
                    return Task.CompletedTask;
                }

                var token = tokenJwt.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();

                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(token))
                {
                    context.Fail("Invalid token format");
                    return Task.CompletedTask;
                }

                var jwt = handler.ReadJwtToken(token);
                var issuer = jwt.Issuer?.Trim();

                if (string.IsNullOrWhiteSpace(issuer))
                {
                    context.Fail("Missing issuer");
                    return Task.CompletedTask;
                }

                var metadataAddress = issuer.TrimEnd('/') + "/.well-known/openid-configuration";

                var cache = context.HttpContext.RequestServices.GetRequiredService<IOidcConfigManagerCache>();

                context.Options.MetadataAddress = metadataAddress;
                context.Options.ConfigurationManager =
                    cache.Get(metadataAddress, context.Options.RequireHttpsMetadata);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                context.HttpContext.Items["AuthError"] = $"Authentication setup failed: {ex.Message}";
                context.HttpContext.Items["AuthStatusCode"] = 401;
                context.Fail($"Authentication setup failed: {ex.Message}");
                return Task.CompletedTask;
            }
        };
    }

    private static Task OnAuthenticationFailed(AuthenticationFailedContext context)
    {
        var errorMessage =
            context.HttpContext.Items["AuthError"] as string
            ?? context.Exception?.Message
            ?? "Authentication failed!";

        var statusCode = context.HttpContext.Items["AuthStatusCode"] as int? ?? 401;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(new { error = errorMessage });
    }

    private static async Task OnChallenge(JwtBearerChallengeContext context)
    {
        if (!context.Response.HasStarted)
        {
            var errorMessage =
                context.HttpContext.Items["AuthError"] as string
                ?? context.ErrorDescription
                ?? "Authentication failed!";

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