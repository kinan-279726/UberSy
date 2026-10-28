namespace UberSy.Contract.Requests.DeliveryPricingRules;

public sealed record UpdatePricingRuleScheduleRequest(TimeOnly? StartHour, TimeOnly? EndHour);
