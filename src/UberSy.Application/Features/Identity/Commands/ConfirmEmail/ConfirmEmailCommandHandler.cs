using MediatR;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(IVerificationService verification)
    : IRequestHandler<ConfirmEmailCommand, Result<Updated>>
{
    private readonly IVerificationService _verification = verification;

    public Task<Result<Updated>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        => _verification.ConfirmEmailAsync(request.Email, request.Code);
}