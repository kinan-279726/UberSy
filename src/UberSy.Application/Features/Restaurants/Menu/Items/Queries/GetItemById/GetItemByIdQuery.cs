using MediatR;
using UberSy.Application.Features.Restaurants.Menu.Items.Dtos;
using UberSy.Domain.Common.Results;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemById;

public sealed record GetItemByIdQuery(Guid ItemId , bool Active = true) : IRequest<Result<ItemDto>>;
