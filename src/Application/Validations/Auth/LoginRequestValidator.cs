using Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validations.Auth;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().MaximumLength(256);
    }
}
