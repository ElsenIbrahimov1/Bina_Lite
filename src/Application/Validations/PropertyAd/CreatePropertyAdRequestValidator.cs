using Application.DTOs.PropertyAd;
using FluentValidation;

namespace Application.Validations.PropertyAd;

public class CreatePropertyAdRequestValidator : AbstractValidator<CreatePropertyAdRequest>
{
    public CreatePropertyAdRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.RoomCount)
            .GreaterThan(0).WithMessage("Room count must be greater than zero.");

        RuleFor(x => x.AreaInSquareMeters)
            .GreaterThan(0).WithMessage("Area must be greater than zero.");

        RuleFor(x => x.OfferType)
            .IsInEnum().WithMessage("Invalid offer type.");

        RuleFor(x => x.PropertyCategory)
            .IsInEnum().WithMessage("Invalid property category.");

        RuleFor(x => x)
            .Must(x => !x.IsMortgage || x.IsExtract)
            .WithMessage("Mortgage properties must have extract.");
    }
}