using NetTopologySuite.Geometries;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Application.Common.Interfaces;

public sealed record DeliveryQuote(decimal DistanceKm, decimal CustomerFee, decimal DriverEarning);

public interface IDeliveryPricingService
{
    Task<Result<DeliveryQuote>> CalculateAsync(Point from, Point to, VehicleType vehicleType, CarType? carType, CancellationToken ct = default);
}
