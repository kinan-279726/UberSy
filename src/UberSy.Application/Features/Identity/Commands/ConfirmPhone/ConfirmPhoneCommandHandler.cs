using MediatR;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Commands.ConfirmPhone;

public sealed class ConfirmPhoneCommandHandler(IUser user, IVerificationService verification)
    : IRequestHandler<ConfirmPhoneCommand, Result<Updated>>
{
    private readonly IUser _user = user;
    private readonly IVerificationService _verification = verification;

    public async Task<Result<Updated>> Handle(ConfirmPhoneCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_user.Id, out var userId)) return ApplicationErrors.Unauthorized;

        return await _verification.ConfirmPhoneAsync(userId, request.Code);
    }
}