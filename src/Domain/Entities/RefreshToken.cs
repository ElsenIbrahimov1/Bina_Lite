using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

public sealed class RefreshToken : BaseEntity<int>
{
    public string Token { get; set; } = string.Empty;

    // Identity user key is string by default
    public string UserId { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public AppUser User { get; set; } = null!;
}