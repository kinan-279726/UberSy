using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UberSy.Domain.Common.Results;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantProfile;

public sealed record UpdateRestaurantProfileCommand(
    Guid RestaurantId,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    string Phone,
    string? Email) : IRequest<Result<Updated>>;
