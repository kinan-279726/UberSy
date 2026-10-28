using MediatR;
using UberSy.Application.Features.Restaurants.Menu.Items.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.requests.CreateItem;

public sealed record CreateItemCommand(
    Guid RestaurantId,
    Guid CategoryId,
    string NameAr,
    string NameEn,
    string DescriptionAr,
    string DescriptionEn,
    decimal Price,
    string ImageUrl,
    int? Calories,
    byte SortOrder)
    : IRequest<Result<ItemDto>>;
