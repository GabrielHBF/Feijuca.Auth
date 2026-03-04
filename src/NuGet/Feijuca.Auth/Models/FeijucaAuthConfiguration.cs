namespace Feijuca.Auth.Models
{
    public sealed record FeijucaAuthConfiguration
    {
        public required string KeycloakUrl { get; init; }
        public required string ApiUrl { get; init; }
    }
}
