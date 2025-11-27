using Feijuca.Auth.Application.Requests.Config;
using Feijuca.Auth.Common.Models;

namespace Feijuca.Auth.Application.Mappers
{
    public static class KeycloakSettingsMapper
    {
        public static KeycloakSettings ToMapper(this AddKeycloakSettingsRequest addKeycloakSettingsRequest)
        {
            return new KeycloakSettings
            {
                Client = addKeycloakSettingsRequest.Client,
                Secrets = addKeycloakSettingsRequest.ClientSecret,
                ServerSettings = addKeycloakSettingsRequest.ServerSettings,
                Realms = [addKeycloakSettingsRequest.Realm]
            };
        }
    }
}
