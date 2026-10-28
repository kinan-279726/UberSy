using MediatR;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;
using UberSy.Domain.Common.Results;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnsByItemId;

public sealed record GetAddOnsByItemIdQuery(Guid ItemId) : IRequest<Result<List<AddOnDto>>>;
