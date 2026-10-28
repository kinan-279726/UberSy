using FluentValidation;

namespace UberSy.Application.Features.ServiceRequests.Commands.OutForDelivery;

public sealed class OutForDelivaryCommandValidator : AbstractValidator<OutForDelivaryCommand>
{
    public OutForDelivaryCommandValidator()
    {
        RuleFor(a => a.ServiceRequestId).NotEmpty();
    }
}
