using Application.Abstracts.Services;
using Application.DTOs.Auth;
using Application.Shared.Helpers.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var (success, error) = await _authService.RegisterAsync(request, ct);

        if (!success)
            return BadRequest(BaseResponse.Fail(error ?? "Register failed."));

        return Ok(BaseResponse.Ok("User registered successfully."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var tokenResponse = await _authService.LoginAsync(request, ct);

        if (tokenResponse is null)
            return Unauthorized(BaseResponse<TokenResponse>.Fail("Invalid login or password.", 401));

        return Ok(BaseResponse<TokenResponse>.Ok(tokenResponse));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest(BaseResponse.Fail("RefreshToken is required."));

        var tokenResponse = await _authService.RefreshTokenAsync(request.RefreshToken, ct);

        if (tokenResponse is null)
            return Unauthorized(BaseResponse<TokenResponse>.Fail("Invalid or expired refresh token.", 401));

        return Ok(BaseResponse<TokenResponse>.Ok(tokenResponse));
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            return BadRequest("userId and token are required.");

        var ok = await _authService.ConfirmEmailAsync(userId, token, ct);

        if (!ok)
            return BadRequest("Token is invalid or expired.");

        return Ok("Email confirmed.");
    }
}