using MediatR;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetRestaurantOrders;

public sealed record GetRestaurantOrdersQuery(
    Guid RestaurantId,
    ServiceRequestStatus? Status,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PagedList<RestaurantOrderListItemDto>>>;
