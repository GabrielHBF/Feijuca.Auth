using Feijuca.Auth.Http.Client;
using Feijuca.Auth.Models;
using Feijuca.Auth.Providers;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
            try
            {
                var endpoint = context.HttpContext.GetEndpoint();
                var hasAuthorize = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null;
                if (!hasAuthorize)
                {
                    context.NoResult();
                    return;
                }

                var tokenJwt = context.Request.Headers.Authorization.FirstOrDefault() ?? context.Request.Query["access_token"].FirstOrDefault();

                if (IsTokenValid(context, tokenJwt).Equals(false))
                {
                    return;
                }

                var token = tokenJwt!.Replace("Bearer ", "");
                var tokenInfos = new JwtSecurityTokenHandler().ReadJwtToken(token);

                if (IsTokenExpirationValid(context, tokenInfos).Equals(false))
                {
                    return;
                }

                IEnumerable<Realm> resolvedRealms;
                if (realms?.Any() ?? false)
                {
                    resolvedRealms = realms;
                }
                else
                {
                    var provider = services.BuildServiceProvider();
                    var authClient = provider.GetRequiredService<IFeijucaAuthClient>();
                    var tenants = await authClient.GetRealmsAsync(token, CancellationToken.None);
                    resolvedRealms = tenants.Data.Select(realm => new Realm
                    {
                        Name = realm.Realm,
                        Issuer = realm.Issuer
                    });
                }

                var tenantName = tokenInfos.Claims.FirstOrDefault(c => c.Type == "tenant")?.Value;
                var tenantRealm = resolvedRealms.FirstOrDefault(realm => realm.Name!.Contains(tenantName!, StringComparison.OrdinalIgnoreCase));

                if (ValidateRealm(context, tenantRealm).Equals(false))
                {
                    return;
                }

                if (IsTokenValidAudience(context, tokenInfos).Equals(false))
                {
                    return;
                }

                if (ValidateIssuer(context, token, tenantRealm!.Issuer!).Equals(false))
                {
                    return;
                }

                var tokenValidationParameters = await GetTokenValidationParameters(token);
                var claims = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out var _);

                context.Principal = claims;
                context.Success();
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                context.HttpContext.Items["AuthError"] = $"Authentication error: {e.Message}";
                await context.Response.WriteAsJsonAsync(new { error = e.Message });
                context.Fail(e.Message);
            }
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

    private static bool IsTokenValid(MessageReceivedContext context, string? tokenJwt)
    {
        if (string.IsNullOrEmpty(tokenJwt))
        {
            context.HttpContext.Items["AuthError"] = "Invalid JWT token!";
            context.HttpContext.Items["AuthStatusCode"] = 401;
            context.Fail("Invalid JWT token!");
            return false;
        }

        return true;
    }

    private static bool IsTokenExpirationValid(MessageReceivedContext context, JwtSecurityToken tokenInfos)
    {
        var expirationClaim = tokenInfos.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        if (expirationClaim != null && long.TryParse(expirationClaim, out var expirationUnix))
        {
            var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expirationUnix).UtcDateTime;
            if (DateTime.UtcNow >= expirationDate)
            {
                context.Response.StatusCode = 401;
                context.HttpContext.Items["AuthError"] = "Token has expired.";
                context.Fail("Token has expired.");
                return false;
            }
        }

        return true;
    }

    private static bool ValidateRealm(MessageReceivedContext context, Realm? tenantRealm)
    {
        if (tenantRealm == null)
        {
            context.HttpContext.Items["AuthError"] = "Invalid realm config provided, please verify!";
            context.HttpContext.Items["AuthStatusCode"] = 401;
            context.Fail("Invalid realm config provided, please verify!");
            return false;
        }
        return true;
    }

    private static bool IsTokenValidAudience(MessageReceivedContext context, JwtSecurityToken tokenInfos)
    {
        var audience = tokenInfos.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
        if (audience != "feijuca-auth-api")
        {
            context.HttpContext.Items["AuthError"] = "Invalid audience, please configure an audience mapper on your realm!";
            context.HttpContext.Items["AuthStatusCode"] = 403;
            context.Fail("Invalid audience!");
            return false;
        }

        return true;
    }


    private static bool ValidateIssuer(MessageReceivedContext context, string jwtToken, string configIssuer)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);

        var issuer = token.Issuer;

        if (configIssuer != issuer)
        {
            context.HttpContext.Items["AuthError"] = "Invalid issuer, please configure an issuer mapper on your realm!";
            context.HttpContext.Items["AuthStatusCode"] = 403;
            context.Fail("Invalid issuer!");
            return false;
        }

        return true;
    }

    private static async Task<TokenValidationParameters> GetTokenValidationParameters(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);

        var issuer = token.Issuer;
        var audience = token.Audiences.FirstOrDefault();

        using var httpClient = new HttpClient();
        var jwksUrl = $"{issuer}/protocol/openid-connect/certs";
        var jwks = await httpClient.GetStringAsync(jwksUrl);
        var jsonWebKeySet = new JsonWebKeySet(jwks);

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = jsonWebKeySet.Keys
        };
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
