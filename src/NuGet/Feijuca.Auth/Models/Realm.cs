using System.Text.Json.Serialization;

namespace Feijuca.Auth.Models;

public class Realm
{
    public string? Name { get; set; }

    public string? Issuer { get; set; }

    [JsonIgnore]
    public string? Audience { get; set; }
}
