using MediatR;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Admin.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.Admin.Queries.Orders;

public sealed record GetAdminOrdersQuery(
    ServiceRequestStatus? Status,
    Guid? RestaurantId,
    Guid? DriverId,
    int? TrackingNumber,
    bool? WaitingForDriver,
    DateOnly? From,
    DateOnly? To,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PagedList<AdminOrderListItemDto>>>;
