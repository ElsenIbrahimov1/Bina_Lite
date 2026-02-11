using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth;

public sealed class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}