using FluentValidation;

namespace UberSy.Application.Features.Admin.Commands.CancelOrder;

public sealed class AdminCancelOrderCommandValidator : AbstractValidator<AdminCancelOrderCommand>
{
    public AdminCancelOrderCommandValidator()
    {
        RuleFor(x => x.ServiceRequestId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(100);
    }
}
