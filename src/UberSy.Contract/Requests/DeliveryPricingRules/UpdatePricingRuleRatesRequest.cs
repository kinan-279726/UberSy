namespace UberSy.Contract.Requests.DeliveryPricingRules;

public sealed record UpdatePricingRuleRatesRequest(
    decimal CustomerRatePerKm, decimal DriverRatePerKm, decimal? DriverRatePerOrder);
