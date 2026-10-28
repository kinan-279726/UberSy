using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Api.Controllers;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Application.Features.Drivers.Queries.GetDriverLocation;
using UberSy.Application.Features.ServiceRequests.Commands.AcceptFoodOrder;
using UberSy.Application.Features.ServiceRequests.Commands.MarkDelivered;
using UberSy.Application.Features.ServiceRequests.Commands.OutForDelivery;
using UberSy.Application.Features.ServiceRequests.Commands.PlaceFoodOrder;
using UberSy.Application.Features.ServiceRequests.Commands.RejectFoodOrder;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Application.Features.ServiceRequests.Queries.GetFoodOrderDetails;
using UberSy.Application.Features.ServiceRequests.Queries.GetMyOrders;
using UberSy.Application.Features.ServiceRequests.Queries.GetOrderQuote;
using UberSy.Contract.Requests.Orders;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers;

[Route("api/v{version:apiVersion}/orders")]
[ApiVersion("1.0")]
public sealed class OrdersController(ISender sender) : ApiController
{
    [HttpPost]
    [Authorize(Policy = Policies.CustomerOnly)]
    [ProducesResponseType(typeof(FoodOrderSummaryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Places a food order.")]
    [EndpointDescription("Creates an order for the current customer. The restaurant is notified in real time.")]
    [EndpointName("PlaceFoodOrder")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Place(
        [FromBody] PlaceFoodOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new PlaceFoodOrderCommand(
            request.RestaurantId,
            request.AddressId,
            request.Items
                .Select(i => new PlaceFoodOrderItem(i.ItemId, i.Quantity, i.AddOnIds?.ToList() ?? []))
                .ToList());

        var result = await sender.Send(
           command,
            cancellationToken);

        return result.Match(
            response => CreatedAtRoute("GetFoodOrderDetails",
                new { version = "1.0", serviceRequestId = response.ServiceRequestId }, response),
            Problem);
    }

    [HttpGet("{serviceRequestId:guid}", Name = "GetFoodOrderDetails")]
    [Authorize]
    [ProducesResponseType(typeof(FoodOrderDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves food order details.")]
    [EndpointDescription("Available to the order's customer, its restaurant owner, its driver and admins.")]
    [EndpointName("GetFoodOrderDetails")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetDetails(Guid serviceRequestId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetFoodOrderDetailsQuery(serviceRequestId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("{serviceRequestId:guid}/accept")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Accepts an order (restaurant).")]
    [EndpointDescription("Moves the order to Preparing and starts dispatching to nearby drivers.")]
    [EndpointName("AcceptOrder")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Accept(Guid serviceRequestId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AcceptOrderCommand(serviceRequestId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{serviceRequestId:guid}/reject")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Rejects an order (restaurant).")]
    [EndpointDescription("Cancels the order with a reason (max 100 characters).")]
    [EndpointName("RejectOrder")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Reject(
        Guid serviceRequestId,
        [FromBody] RejectOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RejectOrderCommand(serviceRequestId, request.Reason), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{serviceRequestId:guid}/out-for-delivery")]
    [Authorize(Policy = Policies.DriverOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Marks the order as picked up.")]
    [EndpointDescription("Only the assigned driver can do this, after collecting the order from the restaurant.")]
    [EndpointName("MarkOrderOutForDelivery")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> OutForDelivery(Guid serviceRequestId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new OutForDelivaryCommand(serviceRequestId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{serviceRequestId:guid}/delivered")]
    [Authorize(Policy = Policies.DriverOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Marks the order as delivered (driver).")]
    [EndpointDescription("Completes the order and deducts the platform share from the driver's wallet.")]
    [EndpointName("MarkOrderDelivered")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> MarkDelivered(Guid serviceRequestId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new MarkDeliveredCommand(serviceRequestId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpGet("mine")]
    [Authorize(Policy = Policies.CustomerOnly)]
    [ProducesResponseType(typeof(PagedList<CustomerOrderListItemDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves the current customer's orders (paged).")]
    [EndpointDescription("Newest first. Optional status filter.")]
    [EndpointName("GetMyOrders")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetMine([FromQuery] ServiceRequestStatus? status,
    [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetMyOrdersQuery(status, pageNumber, pageSize), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("{serviceRequestId:guid}/driver-location")]
    [Authorize(Policy = Policies.CustomerOnly)]
    [ProducesResponseType(typeof(DriverLocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves the assigned driver's current location.")]
    [EndpointDescription("Use once when the tracking screen opens; live updates arrive on UserHub as DriverLocationUpdated.")]
    [EndpointName("GetDriverLocation")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetDriverLocation(Guid serviceRequestId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetDriverLocationQuery(serviceRequestId), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("quote")]
    [Authorize(Policy = Policies.CustomerOnly)]
    [ProducesResponseType(typeof(OrderQuoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Previews the order price before placing it.")]
    [EndpointDescription("Runs the same validation as placing the order but creates nothing. Returns items total, tax, estimated delivery fee and the estimated grand total. The delivery fee is final only when a driver accepts.")]
    [EndpointName("GetOrderQuote")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Quote(
    [FromBody] PlaceFoodOrderRequest request,
    CancellationToken cancellationToken)
    {
        var query = new GetOrderQuoteQuery(
            request.RestaurantId,
            request.AddressId,
            request.Items
                .Select(i => new PlaceFoodOrderItem(i.ItemId, i.Quantity, i.AddOnIds?.ToList() ?? []))
                .ToList());

        var result = await sender.Send(query, cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }
}