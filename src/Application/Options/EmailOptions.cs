using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Options;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public bool Enabled { get; set; } = false;

    // SMTP
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;

    public string? UserName { get; set; }
    public string? Password { get; set; }

    // Sender
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;

    // Confirm URL base (should call your confirm endpoint directly)
    // Example: https://localhost:7243/api/auth/confirm-email
    public string ConfirmEmailBaseUrl { get; set; } = string.Empty;
}
