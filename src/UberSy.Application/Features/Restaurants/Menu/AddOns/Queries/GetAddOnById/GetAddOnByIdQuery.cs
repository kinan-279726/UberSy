using MediatR;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnById;

public sealed record GetAddOnByIdQuery(Guid AddOnId) : IRequest<Result<AddOnDto>>;
