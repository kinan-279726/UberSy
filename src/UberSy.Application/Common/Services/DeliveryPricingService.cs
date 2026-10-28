using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Helpers;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Application.Common.Services;

public sealed class DeliveryPricingService(
    IApplicationDbContext context,
    IRoutingService routing,
    IConfiguration configuration,
    ILogger<DeliveryPricingService> logger) : IDeliveryPricingService
{
    private readonly IApplicationDbContext _context = context;
    private readonly IRoutingService _routing = routing;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<DeliveryPricingService> _logger = logger;

    public async Task<Result<DeliveryQuote>> CalculateAsync(
        Point from, Point to, VehicleType vehicleType, CarType? carType, CancellationToken ct = default)
    {
        var distance = await _routing.GetDistanceAsync(from, to, ct);
        var distanceKm = distance.DistanceKm;

        var now = DamascusTime.Now();

        var rule = await _context.TbDeliveryPricingRules
            .AsNoTracking()
            .Where(r => r.IsActive
                        && !r.IsDeleted
                        && r.VehicleType == vehicleType
                        && r.ServiceTier == ServiceTier.Standard
                        && r.CarType == carType
                        && (r.StartHour == null
                            || (r.StartHour <= now && r.EndHour > now)))
            .OrderByDescending(r => r.StartHour != null)
            .FirstOrDefaultAsync(ct);

        if (rule is not null)
        {
            if (rule.MaxDistanceKm is not null && distanceKm > rule.MaxDistanceKm)
            {
                return ApplicationErrors.DeliveryDistanceTooLong;
            }

            return new DeliveryQuote(
                DistanceKm: distanceKm,
                CustomerFee: Math.Round(distanceKm * rule.CustomerRatePerKm, 2),
                DriverEarning: Math.Round(distanceKm * rule.DriverRatePerKm, 2));
        }

        return CalculateWithDefault(distanceKm, vehicleType, carType, now);
    }

    private Result<DeliveryQuote> CalculateWithDefault(
        decimal distanceKm, VehicleType vehicleType, CarType? carType, TimeOnly now)
    {
        var section = _configuration.GetSection("DeliveryPricing:Default");

        var customerRate = section.GetValue<decimal?>("CustomerRatePerKm");
        var driverRate = section.GetValue<decimal?>("DriverRatePerKm");

        if (customerRate is null || driverRate is null)
        {
            _logger.LogError(
                "No Pricing Rule For {Vehicle}/{Car} At {Time} And No Default Configured.",
                vehicleType, carType, now);
            return ApplicationErrors.DeliveryPricingRuleNotFound;
        }

        var maxDistanceKm = section.GetValue<decimal?>("MaxDistanceKm");
        if (maxDistanceKm is not null && distanceKm > maxDistanceKm)
        {
            return ApplicationErrors.DeliveryDistanceTooLong;
        }

        _logger.LogWarning(
            "No Pricing Rule For {Vehicle}/{Car} At {Time}. Using Default Rates.",
            vehicleType, carType, now);

        return new DeliveryQuote(
            DistanceKm: distanceKm,
            CustomerFee: Math.Round(distanceKm * customerRate.Value, 2),
            DriverEarning: Math.Round(distanceKm * driverRate.Value, 2));
    }
}