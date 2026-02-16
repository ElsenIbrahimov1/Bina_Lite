using Application.Abstracts.Services;
using Application.DTOs.Auth;
using Application.Options;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;

namespace Persistence.Services;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly JwtOptions _jwt;

    // ✅ NEW
    private readonly IEmailSender _emailSender;
    private readonly EmailOptions _emailOpt;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtTokenGenerator jwtGenerator,
        IRefreshTokenService refreshTokenService,
        IOptions<JwtOptions> jwtOptions,
        IEmailSender emailSender,                 // ✅ NEW
        IOptions<EmailOptions> emailOptions)      // ✅ NEW
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        _refreshTokenService = refreshTokenService;
        _jwt = jwtOptions.Value;

        _emailSender = emailSender;
        _emailOpt = emailOptions.Value;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var user = new AppUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName,

            // ✅ CHANGED: must confirm by email
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        // ✅ default role User
        var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.User);
        if (!roleResult.Succeeded)
            return (false, string.Join("; ", roleResult.Errors.Select(e => e.Description)));

        // ✅ Generate confirm token and send email
        // (only if user has email)
        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            var rawToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // URL-safe token
            var tokenBytes = Encoding.UTF8.GetBytes(rawToken);
            var encodedToken = WebEncoders.Base64UrlEncode(tokenBytes);

            // Build confirm link: baseUrl?userId=...&token=...
            var baseUrl = (_emailOpt.ConfirmEmailBaseUrl ?? string.Empty).TrimEnd('/');

            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                var confirmLink =
                    $"{baseUrl}?userId={Uri.EscapeDataString(user.Id)}&token={Uri.EscapeDataString(encodedToken)}";

                var subject = "Confirm your email";
                var htmlBody = $"""
<p>Welcome to BinaLite!</p>
<p>Please confirm your email by clicking the link below:</p>
<p><a href="{confirmLink}">Confirm email</a></p>
""";

                var textBody = $"Please confirm your email: {confirmLink}";

                await _emailSender.SendAsync(user.Email, subject, htmlBody, textBody, ct);
            }
        }

        return (true, null);
    }

    public async Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Login)
                   ?? await _userManager.FindByNameAsync(request.Login);

        if (user is null) return null;

        var check = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!check.Succeeded) return null;

        // ✅ NEW: block login if email not confirmed
        if (!user.EmailConfirmed)
            throw new Application.Shared.Helpers.Exceptions.BadRequestException(
                "Email is not confirmed. Please confirm your email using the link sent to your inbox.");

        return await BuildTokenResponseAsync(user, ct);
    }

    public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var user = await _refreshTokenService.ValidateAndConsumeAsync(refreshToken, ct);
        if (user is null) return null;

        // optional: also enforce email confirmation for refresh
        if (!user.EmailConfirmed)
            throw new Application.Shared.Helpers.Exceptions.BadRequestException(
                "Email is not confirmed. Please confirm your email first.");

        return await BuildTokenResponseAsync(user, ct);
    }

    // ✅ NEW: confirm email endpoint uses this
    public async Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) return false;
        if (string.IsNullOrWhiteSpace(token)) return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return false;

        string decodedToken;
        try
        {
            var tokenBytes = WebEncoders.Base64UrlDecode(token);
            decodedToken = Encoding.UTF8.GetString(tokenBytes);
        }
        catch
        {
            return false;
        }

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        return result.Succeeded;
    }

    private async Task<TokenResponse> BuildTokenResponseAsync(AppUser user, CancellationToken ct)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _jwtGenerator.GenerateAccessToken(user, roles);

        var newRefresh = await _refreshTokenService.CreateAsync(user, ct);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefresh,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes)
        };
    }
}