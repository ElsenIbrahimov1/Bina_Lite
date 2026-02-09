using Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Validations.Auth;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().MinimumLength(3).MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(256);

        RuleFor(x => x.FullName)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(8).MaximumLength(100)
            .Must(HasDigit).WithMessage("Password must contain at least one digit.")
            .Must(HasLower).WithMessage("Password must contain at least one lowercase letter.")
            .Must(HasUpper).WithMessage("Password must contain at least one uppercase letter.")
            .Must(HasSpecial).WithMessage("Password must contain at least one special character.");
    }

    private static bool HasDigit(string p) => p.Any(char.IsDigit);
    private static bool HasLower(string p) => p.Any(char.IsLower);
    private static bool HasUpper(string p) => p.Any(char.IsUpper);
    private static bool HasSpecial(string p) => Regex.IsMatch(p, @"[^a-zA-Z0-9]");
}
