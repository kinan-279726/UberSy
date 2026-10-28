using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.Drivers.Commands.ApproveDriver;

public sealed class ApproveDriverCommandHandler(
    IApplicationDbContext context,
    IUserRoleService userRoles,
    ILogger<ApproveDriverCommandHandler> logger,
    IUser user) : IRequestHandler<ApproveDriverCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUserRoleService _userRoles = userRoles;
    private readonly ILogger<ApproveDriverCommandHandler> _logger = logger;
    private readonly IUser _user = user;

    public async Task<Result<Updated>> Handle(ApproveDriverCommand request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin))
        {
            return ApplicationErrors.AccessDenied;
        }

        var driver = await _context.TbDrivers
            .FirstOrDefaultAsync(d => d.Id == request.DriverId, cancellationToken);
        if (driver is null) return ApplicationErrors.DriverNotFound;

        if (!driver.IsApproved)
        {
            if (request.ServiceTier != driver.ServiceTier)
            {
                var tierResult = driver.UpdateVehicle(
                    driver.VehicleType, request.ServiceTier, driver.CarType,
                    driver.LicenseNumber, driver.PlateNumber, driver.Model, driver.Color);

            }

            var approveResult = driver.Approve();
            if (approveResult.IsError) return approveResult.Errors;

            await _context.SaveChangesAsync(cancellationToken);
        }
        var roleResult = await _userRoles.AddToRoleAsync(driver.UserId, Roles.Driver);
        if (roleResult.IsError && roleResult.TopError.Code != ApplicationErrors.RoleAlreadyAssigned.Code)
        {
            _logger.LogError(
                "Driver '{DriverId}' Approved But Role Assignment Failed: {Error}. Approve Again To Retry.",
                driver.Id, roleResult.TopError.Description);
            return roleResult.Errors;
        }

        _logger.LogInformation("Driver '{DriverId}' Approved By Admin '{AdminId}'.", driver.Id, _user.Id);

        return Result.Updated;
    }
}