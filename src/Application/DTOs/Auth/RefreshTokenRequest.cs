using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth;

public sealed class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

