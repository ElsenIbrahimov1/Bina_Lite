using Application.DTOs.District;
using FluentValidation;

namespace Application.Validations.District;

public class CreateDistrictRequestValidator : AbstractValidator<CreateDistrictRequest>
{
    public CreateDistrictRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("CityId must be greater than zero.");
    }
}
