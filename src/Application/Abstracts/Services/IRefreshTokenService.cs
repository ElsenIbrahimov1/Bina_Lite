using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Abstracts.Services;

public interface IRefreshTokenService
{
    Task<string> CreateAsync(AppUser user, CancellationToken ct = default);

    /// <summary>
    /// If token is valid -> consume (delete) and return user, else null.
    /// </summary>
    Task<AppUser?> ValidateAndConsumeAsync(string refreshToken, CancellationToken ct = default);
}