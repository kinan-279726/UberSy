using FluentValidation;

namespace UberSy.Application.Features.ServiceRequests.Commands.AcceptFoodOrder;

public sealed class AcceptOrderCommandValidator : AbstractValidator<AcceptOrderCommand>
{
    public AcceptOrderCommandValidator()
    {
        RuleFor(x => x.ServiceRequestId).NotEmpty();
    }
}