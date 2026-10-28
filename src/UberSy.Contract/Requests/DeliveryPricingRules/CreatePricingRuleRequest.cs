namespace UberSy.Contract.Requests.DeliveryPricingRules;

public sealed record CreatePricingRuleRequest(
    byte VehicleType, byte ServiceTier, byte? CarType,
    decimal CustomerRatePerKm, decimal DriverRatePerKm, decimal? DriverRatePerOrder,
    decimal? MaxDistanceKm, TimeOnly? StartHour, TimeOnly? EndHour);
