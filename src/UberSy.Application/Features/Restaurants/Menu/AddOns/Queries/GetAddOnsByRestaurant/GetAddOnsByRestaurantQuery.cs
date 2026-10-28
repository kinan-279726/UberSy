using MediatR;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnsByRestaurant;

public sealed record GetAddOnsByRestaurantQuery(Guid RestaurantId) : IRequest<Result<List<AddOnDto>>>;
