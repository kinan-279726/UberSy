using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Admin.Commands.CancelOrder;
using UberSy.Application.Features.Admin.Commands.UnassignDriver;
using UberSy.Application.Features.Admin.Dtos;
using UberSy.Application.Features.Admin.Queries.OrderDetails;
using UberSy.Application.Features.Admin.Queries.Orders;
using UberSy.Contract.Requests.Admin;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers.Admin;

[Route("api/v{version:apiVersion}/admin/orders")]
[ApiVersion("1.0")]
[Authorize(Policy = Policies.AdminOnly)]
public sealed class AdminOrdersController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<AdminOrderListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Retrieves all orders (paged).")]
    [EndpointDescription("Filter by status, restaurant, driver, tracking number, date range, or orders waiting for a driver.")]
    [EndpointName("GetAdminOrders")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAll(
        [FromQuery] ServiceRequestStatus? status,
        [FromQuery] Guid? restaurantId,
        [FromQuery] Guid? driverId,
        [FromQuery] int? trackingNumber,
        [FromQuery] bool? waitingForDriver,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminOrdersQuery(
            status, restaurantId, driverId, trackingNumber, waitingForDriver, from, to, pageNumber, pageSize);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("{serviceRequestId:guid}")]
    [ProducesResponseType(typeof(AdminOrderDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves an order's full details.")]
    [EndpointDescription("Restaurant, customer, driver, items, money breakdown, history and offers.")]
    [EndpointName("GetAdminOrderDetails")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetById(Guid serviceRequestId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAdminOrderDetailsQuery(serviceRequestId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("{serviceRequestId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Cancels an order (admin).")]
    [EndpointDescription("Allowed in any non-final status. Frees the assigned driver.")]
    [EndpointName("AdminCancelOrder")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Cancel(
        Guid serviceRequestId,
        [FromBody] AdminCancelOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AdminCancelOrderCommand(serviceRequestId, request.Reason), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{serviceRequestId:guid}/unassign-driver")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Unassigns the driver from an order.")]
    [EndpointDescription("Only before pickup. The order is offered to other drivers.")]
    [EndpointName("AdminUnassignDriver")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UnassignDriver(Guid serviceRequestId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AdminUnassignDriverCommand(serviceRequestId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}