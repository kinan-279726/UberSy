using MediatR;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Commands.PlaceFoodOrder;

public sealed record PlaceFoodOrderItem(Guid ItemId, int Quantity, List<Guid> AddOnIds);

public sealed record PlaceFoodOrderCommand(
    Guid RestaurantId,
    Guid AddressId,
    List<PlaceFoodOrderItem> Items)
    : IRequest<Result<FoodOrderSummaryDto>>;