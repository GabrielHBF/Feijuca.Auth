using Feijuca.Auth.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Feijuca.Auth.Validators;

public static class TenantTokenValidator
{
    public static async Task ProcessTokenValidationAsync(MessageReceivedContext context, string token, IOpenIdConfigurationProvider openIdProvider)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenInfos = tokenHandler.ReadJwtToken(token);

        if (!IsTokenExpirationValid(context, tokenInfos) || !IsTokenValidAudience(context, tokenInfos))
        {
            return;
        }

        try
        {
            var parameters = await GetValidationParametersAsync(tokenInfos, openIdProvider);
            var claims = tokenHandler.ValidateToken(token, parameters, out var _);

            context.Principal = claims;
            context.Success();
        }
        catch (SecurityTokenSignatureKeyNotFoundException)
        {
            openIdProvider.RequestRefresh(tokenInfos.Issuer);

            try
            {
                var parameters = await GetValidationParametersAsync(tokenInfos, openIdProvider);
                var claims = tokenHandler.ValidateToken(token, parameters, out var _);

                context.Principal = claims;
                context.Success();
            }
            catch (Exception ex)
            {
                FailContext(context, 401, $"Invalid token signature after key refresh: {ex.Message}");
            }
        }
        catch (Exception e)
        {
            FailContext(context, 401, $"Authentication error: {e.Message}");
        }
    }

    private static async Task<TokenValidationParameters> GetValidationParametersAsync(JwtSecurityToken token, IOpenIdConfigurationProvider openIdProvider)
    {
        var issuer = token.Issuer;
        var audience = token.Audiences.FirstOrDefault();

        var config = await openIdProvider.GetAsync(issuer, CancellationToken.None);

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = config.SigningKeys
        };
    }

    private static bool IsTokenExpirationValid(MessageReceivedContext context, JwtSecurityToken tokenInfos)
    {
        var expirationClaim = tokenInfos.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        if (expirationClaim != null && long.TryParse(expirationClaim, out var expirationUnix))
        {
            var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expirationUnix).UtcDateTime;
            if (DateTime.UtcNow >= expirationDate)
            {
                FailContext(context, 401, "Token has expired.");
                return false;
            }
        }
        return true;
    }

    private static bool IsTokenValidAudience(MessageReceivedContext context, JwtSecurityToken tokenInfos)
    {
        var audience = tokenInfos.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
        if (audience != "feijuca-auth-api")
        {
            FailContext(context, 403, "Invalid audience, please configure an audience mapper on your realm!");
            return false;
        }
        return true;
    }

    private static void FailContext(MessageReceivedContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.HttpContext.Items["AuthError"] = message;
        context.Fail(message);
    }
}