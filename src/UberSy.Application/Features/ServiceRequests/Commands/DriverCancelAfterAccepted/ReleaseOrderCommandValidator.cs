using FluentValidation;

namespace UberSy.Application.Features.ServiceRequests.Commands.DriverCancelAfterAccepted;

public sealed class ReleaseOrderCommandValidator : AbstractValidator<ReleaseOrderCommand>
{
    public ReleaseOrderCommandValidator()
    {
        RuleFor(x => x.ServiceRequestId).NotEmpty();
    }
}
