using MediatR;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Queries.GetUserInfo;

public class GetUserByIdQueryHandler(ILogger<GetUserByIdQueryHandler> logger, IIdentityService identityService)
    : IRequestHandler<GetUserByIdQuery, Result<ApplicationUserDto>>
{
    private readonly ILogger<GetUserByIdQueryHandler> _logger = logger;
    private readonly IIdentityService _identityService = identityService;

    public async Task<Result<ApplicationUserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var getUserByIdResult = await _identityService.GetUserByIdAsync(Guid.Parse(request.UserId!));

        if (getUserByIdResult.IsError)
        {
            _logger.LogError("User with Id { UserId }{ErrorDetails}", request.UserId, getUserByIdResult.TopError.Description);

            return getUserByIdResult.Errors;
        }

        return getUserByIdResult.Value;
    }
}
