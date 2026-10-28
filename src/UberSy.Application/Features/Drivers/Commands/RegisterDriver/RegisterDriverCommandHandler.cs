using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Application.Features.Drivers.Mappers;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Wallet;

namespace UberSy.Application.Features.Drivers.Commands.RegisterDriver;

public sealed class RegisterDriverCommandHandler(
    IApplicationDbContext context,
    IOwnershipValidator ownership,
    IDataProtectionProvider protectionProvider,
    ILogger<RegisterDriverCommandHandler> logger)
    : IRequestHandler<RegisterDriverCommand, Result<DriverDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly IDataProtector _protector = protectionProvider.CreateProtector("DriverInfo.Protection");
    private readonly ILogger<RegisterDriverCommandHandler> _logger = logger;



    public async Task<Result<DriverDto>> Handle(RegisterDriverCommand request, CancellationToken cancellationToken)
    {
        var user = await _ownership.IsUser();

        if (user.IsError) return user.Errors;

        var userId = user.Value;

        if (await _context.TbDrivers.AsNoTracking().AnyAsync(d => d.UserId == userId, cancellationToken))
        {
            return ApplicationErrors.DriverAlreadyRegistered;
        }

        var nationalId = request.NationalId.Trim();

        if (await _context.TbDrivers.AsNoTracking().AnyAsync(d => d.NationalId == nationalId, cancellationToken))
        {
            return ApplicationErrors.NationalIdAlreadyRegistered;
        }

        var driverResult = TbDriver.Create(
            id: Guid.NewGuid(),
            userId: userId,
            vehicleType: request.VehicleType,
            serviceTier: ServiceTier.Standard,
            carType: request.CarType,
            nationalId: nationalId,
            photoUrl: request.PhotoUrl,
            licenseNumber: request.LicenseNumber, // _protector.Protect(request.LicenseNumber!),
            plateNumber: request.PlateNumber, //_protector.Protect(request.PlateNumber!),
            model: request.Model,
            color: request.Color);

        if (driverResult.IsError)
        {
            _logger.LogWarning("Driver Registration Failed For User '{UserId}': {Error}", userId, driverResult.TopError.Description);
            return driverResult.Errors;
        }

        var driver = driverResult.Value;

        var walletResult = TbDriverWallet.Create(Guid.NewGuid(), driver.Id);
        if (walletResult.IsError)
        {
            return walletResult.Errors;
        }

        _context.TbDrivers.Add(driver);
        _context.TbDriverWallets.Add(walletResult.Value);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Driver '{DriverId}' Registered For User '{UserId}' (Pending Approval).", driver.Id, userId);

        return driver.ToDto();
    }
}