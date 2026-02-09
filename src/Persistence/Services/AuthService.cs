using Application.Abstracts.Services;
using Application.DTOs.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Services;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtTokenGenerator _jwt;

    public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtTokenGenerator jwt)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var user = new AppUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded) return (true, null);

        var error = string.Join("; ", result.Errors.Select(e => e.Description));
        return (false, error);
    }

    public async Task<string?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Login)
                   ?? await _userManager.FindByNameAsync(request.Login);

        if (user is null) return null;

        var ok = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!ok.Succeeded) return null;

        return _jwt.GenerateToken(user);
    }
}