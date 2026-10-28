using MediatR;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.CreateRestaurant;

public sealed record CreateRestaurantCommand(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    string Phone,
    string? Email,
    decimal CommissionRate,
    decimal TaxRate,
    TimeOnly OpeningTime,
    TimeOnly ClosingTime,
    decimal MinOrderAmount,
    decimal MaxOrderAmount,
    double Latitude,
    double Longitude)
    : IRequest<Result<RestaurantDto>>;
