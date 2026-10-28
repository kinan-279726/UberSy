using FluentValidation;

public sealed class AssignPrinterToCategoryCommandValidator : AbstractValidator<AssignPrinterToCategoryCommand>
{
    public AssignPrinterToCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.PrinterId).NotEmpty();
    }
}
