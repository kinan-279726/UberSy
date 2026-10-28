using MediatR;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Commands.SendPhoneCode;

public sealed class SendPhoneCodeCommandHandler(IUser user, IVerificationService verification)
    : IRequestHandler<SendPhoneCodeCommand, Result<Updated>>
{
    private readonly IUser _user = user;
    private readonly IVerificationService _verification = verification;

    public async Task<Result<Updated>> Handle(SendPhoneCodeCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_user.Id, out var userId)) return ApplicationErrors.Unauthorized;

        return await _verification.SendPhoneCodeAsync(userId, cancellationToken);
    }
}