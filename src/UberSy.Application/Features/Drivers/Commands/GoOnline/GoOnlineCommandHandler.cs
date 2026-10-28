using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Commands.GoOnline;

public sealed class GoOnlineCommandHandler(IApplicationDbContext context, IUser user) : IRequestHandler<GoOnlineCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;

    public async Task<Result<Updated>> Handle(GoOnlineCommand request, CancellationToken cancellationToken)
    {
        var isDriver = _user.IsInRole(Domain.Identity.Roles.Driver);

        var userId = _user.Id;

        if (!isDriver || userId is null) return ApplicationErrors.AccessDenied;

        var driver = await _context.TbDrivers.FirstOrDefaultAsync(d => d.UserId == Guid.Parse(userId), cancellationToken);

        if (driver is null) return ApplicationErrors.DriverNotFound;

        if (!driver.IsApproved) return ApplicationErrors.DriverNotApproved;

        var onlineResult = driver.GoOnline();

        if (onlineResult.IsError) return onlineResult.Errors;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}

