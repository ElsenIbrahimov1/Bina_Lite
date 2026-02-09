using Application.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Options;

public sealed class ConfigureJwtBearerOptions : IConfigureOptions<JwtBearerOptions>
{
    private readonly JwtOptions _opt;

    public ConfigureJwtBearerOptions(IOptions<JwtOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = _opt.Issuer,
            ValidAudience = _opt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    }
}