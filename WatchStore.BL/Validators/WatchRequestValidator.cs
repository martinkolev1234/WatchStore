using FluentValidation;
using WatchStore.Core;
using WatchStore.Core.Requests;

namespace WatchStore.BL.Validators;

public class CreateWatchRequestValidator : AbstractValidator<CreateWatchRequest>
{
    public CreateWatchRequestValidator()
    {
        RuleFor(x => x.Brand).NotEmpty().Length(2, 30);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThan(0);
        
        RuleFor(x => x.ProductionYear)
            .Must(y => y >= 1900 && y <= DateTime.Now.Year + 1)
            .WithMessage(x => $"Year must be between 1900 and {DateTime.Now.Year + 1}.");

        RuleFor(x => x.CaseDiameterMm).InclusiveBetween(20, 60);
    }
}

public class UpdateWatchRequestValidator : AbstractValidator<UpdateWatchRequest>
{
    public UpdateWatchRequestValidator()
    {
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .Length(2, 30);

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(50);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.ProductionYear)
            .Must(y => y >= 1900 && y <= DateTime.Now.Year + 1)
            .WithMessage($"Year must be between 1900 and {DateTime.Now.Year + 1}.");

        RuleFor(x => x.CaseDiameterMm)
            .InclusiveBetween(20, 60).WithMessage("Diameter must be between 20mm and 60mm.");
    }
}