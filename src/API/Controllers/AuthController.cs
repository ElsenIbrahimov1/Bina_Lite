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
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var (success, error) = await _auth.RegisterAsync(request, ct);
        if (!success)
            return BadRequest(BaseResponse.Fail(error ?? "Register failed."));

        return Ok(BaseResponse.Ok("Registered successfully."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var token = await _auth.LoginAsync(request, ct);
        if (token is null)
            return Unauthorized(BaseResponse<string>.Fail("Invalid login or password.", 401));

        return Ok(BaseResponse<string>.Ok(token, "OK"));
    }
}