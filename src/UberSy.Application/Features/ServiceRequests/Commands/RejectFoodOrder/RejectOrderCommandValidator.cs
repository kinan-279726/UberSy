using FluentValidation;

namespace UberSy.Application.Features.ServiceRequests.Commands.RejectFoodOrder;

public sealed class RejectOrderCommandValidator : AbstractValidator<RejectOrderCommand>
{
    public RejectOrderCommandValidator()
    {
        RuleFor(x => x.ServiceRequestId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(100);
    }
}