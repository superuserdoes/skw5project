namespace DeliFHery.Api.Security;

public class KeycloakOptions
{
    public const string SectionName = "Keycloak";

    public string? Authority { get; set; }

    public string? Audience { get; set; }

    public string? RequiredScope { get; set; }

    public bool RequireHttpsMetadata { get; set; } = true;
}
