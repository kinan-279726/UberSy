using MediatR;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;
using UberSy.Domain.Common.Results;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.CreateAddOn;

public sealed record CreateAddOnCommand(Guid RestaurantId,
    string NameAr,
    string NameEn,
    decimal Price)
    : IRequest<Result<AddOnDto>>;
