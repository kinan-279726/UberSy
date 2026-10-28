using FluentValidation;

namespace UberSy.Application.Features.Drivers.Commands.RegisterDriver;

public sealed class RegisterDriverCommandValidator : AbstractValidator<RegisterDriverCommand>
{
    public RegisterDriverCommandValidator()
    {
        RuleFor(x => x.VehicleType).IsInEnum();
        RuleFor(x => x.CarType).IsInEnum();
        RuleFor(x => x.NationalId).NotEmpty().MaximumLength(20);
        RuleFor(x => x.PhotoUrl).MaximumLength(1000);
        RuleFor(x => x.LicenseNumber).MaximumLength(50);
        RuleFor(x => x.PlateNumber).MaximumLength(20);
        RuleFor(x => x.Model).MaximumLength(100);
        RuleFor(x => x.Color).MaximumLength(100);
    }
}