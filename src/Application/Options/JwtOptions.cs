namespace Application.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;

    // Access token lifetime
    public int ExpirationMinutes { get; set; } = 60;

    // Refresh token lifetime (default 7 days)
    public int RefreshExpirationMinutes { get; set; } = 10080;
}