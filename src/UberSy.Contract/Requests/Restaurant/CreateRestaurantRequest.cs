namespace UberSy.Contract.Requests.Restaurant;
public sealed record CreateRestaurantRequest(
    string NameAr, string NameEn, string? DescriptionAr, string? DescriptionEn,
    string Phone, string? Email, decimal CommissionRate, decimal TaxRate,
    TimeOnly OpeningTime, TimeOnly ClosingTime,
    decimal MinOrderAmount, decimal MaxOrderAmount,
    double Latitude, double Longitude);