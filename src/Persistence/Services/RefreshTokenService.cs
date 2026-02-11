using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Options;
using Domain.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Persistence.Services;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _repo;
    private readonly JwtOptions _jwt;

    public RefreshTokenService(IRefreshTokenRepository repo, IOptions<JwtOptions> jwtOptions)
    {
        _repo = repo;
        _jwt = jwtOptions.Value;
    }

    public async Task<string> CreateAsync(AppUser user, CancellationToken ct = default)
    {
        // 32 bytes -> 64 hex chars
        var bytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToHexString(bytes).ToLowerInvariant();

        var now = DateTime.UtcNow;

        var entity = new RefreshToken
        {
            Token = token,
            UserId = user.Id,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddMinutes(_jwt.RefreshExpirationMinutes)
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return token;
    }

    public async Task<AppUser?> ValidateAndConsumeAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        var entity = await _repo.GetByTokenWithUserAsync(refreshToken, ct);

        if (entity is null)
            return null;

        if (entity.ExpiresAtUtc <= DateTime.UtcNow)
            return null;

        // consume (one-time use)
        await _repo.DeleteByTokenAsync(refreshToken, ct);

        return entity.User;
    }
}