using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Queries.GetMyDriverProfile;

public sealed class GetMyDriverProfileQueryHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<GetMyDriverProfileQuery, Result<DriverProfileDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<DriverProfileDto>> Handle(
        GetMyDriverProfileQuery request, CancellationToken cancellationToken)
    {
        var driverResult = await _ownership.IsDriver();
        if (driverResult.IsError) return driverResult.Errors;
        var driverId = driverResult.Value;

        var profile = await _context.TbDrivers
            .AsNoTracking()
            .Where(d => d.Id == driverId)
            .Select(d => new DriverProfileDto
            {
                Id = d.Id,
                VehicleType = d.VehicleType,
                ServiceTier = d.ServiceTier,
                CarType = d.CarType,
                PhotoUrl = d.PhotoUrl,
                LicenseNumber = d.LicenseNumber,
                PlateNumber = d.PlateNumber,
                Model = d.Model,
                Color = d.Color,
                Status = d.Status,
                IsApproved = d.IsApproved,
                WalletBalance = d.DriverWallet != null ? d.DriverWallet.Balance : 0m,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (profile is null) return ApplicationErrors.DriverNotFound;

        return profile;
    }
}
