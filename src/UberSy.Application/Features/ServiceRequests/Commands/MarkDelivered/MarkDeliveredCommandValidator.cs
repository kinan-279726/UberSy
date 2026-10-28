using FluentValidation;

namespace UberSy.Application.Features.ServiceRequests.Commands.MarkDelivered;

public sealed class MarkDeliveredCommandValidator : AbstractValidator<MarkDeliveredCommand>
{
    public MarkDeliveredCommandValidator() => RuleFor(x => x.ServiceRequestId).NotEmpty();
}