using FluentValidation;

namespace UberSy.Application.Features.ServiceRequests.Commands.CancelService;

public sealed class CancelServiceCommandValidator : AbstractValidator<CancelServiceCommand>
{
    public CancelServiceCommandValidator()
    {
        RuleFor(x => x.ServiceId).NotEmpty();
    }
}
