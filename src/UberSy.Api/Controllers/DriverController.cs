using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Drivers.Commands.GoOffline;
using UberSy.Application.Features.Drivers.Commands.GoOnline;
using UberSy.Application.Features.Drivers.Commands.RegisterDriver;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Application.Features.Drivers.Queries.GetMyDriverProfile;
using UberSy.Application.Features.Drivers.Queries.MyActiveOrder;
using UberSy.Application.Features.Drivers.Queries.MyWallet;
using UberSy.Application.Features.Drivers.Queries.MyWalletTransactions;
using UberSy.Application.Features.ServiceRequests.Commands.DriverCancelAfterAccepted;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Application.Features.ServiceRequests.Queries.GetMyDriverOrders;
using UberSy.Contract.Requests.Drivers;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers.Wallet.Enums;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers;

[Route("api/v{version:apiVersion}/drivers")]
[ApiVersion("1.0")]
public sealed class DriversController(ISender sender) : ApiController
{
    [HttpPost("register")]
    [Authorize(Policy = Policies.CustomerOnly)]
    [ProducesResponseType(typeof(DriverDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Registers the current user as a driver.")]
    [EndpointDescription("Creates a pending driver profile and an empty wallet. An admin must approve it.")]
    [EndpointName("RegisterDriver")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDriverRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterDriverCommand(
           (VehicleType) request.VehicleType,
           request.CarType is null ? null :((CarType) request.CarType),
           request.NationalId, request.PhotoUrl,
            request.LicenseNumber,
            request.PlateNumber,
            request.Model,
            request.Color);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(response => StatusCode(StatusCodes.Status201Created, response), Problem);
    }

    [HttpPost("online")]
    [Authorize(Policy = Policies.DriverOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Sets the driver online.")]
    [EndpointDescription("The driver starts receiving order offers. Requires an approved account.")]
    [EndpointName("GoOnline")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GoOnline(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GoOnlineCommand(), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("offline")]
    [Authorize(Policy = Policies.DriverOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Sets the driver offline.")]
    [EndpointDescription("The driver stops receiving order offers.")]
    [EndpointName("GoOffline")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GoOffline(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GoOfflineCommand(), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpGet("orders")]
    [Authorize(Policy = Policies.DriverOnly)]
    [ProducesResponseType(typeof(PagedList<DriverOrderListItemDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves the current driver's orders (paged).")]
    [EndpointDescription("Newest first, with the cash to collect and the driver's earning. Optional status filter.")]
    [EndpointName("GetMyDriverOrders")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetOrders([FromQuery] ServiceRequestStatus? status,
    [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetMyDriverOrdersQuery(status, pageNumber, pageSize), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("me")]
    [Authorize(Policy = Policies.DriverOnly)]
    [ProducesResponseType(typeof(DriverProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves the current driver's profile.")]
    [EndpointDescription("Vehicle data, status, approval state and wallet balance.")]
    [EndpointName("GetMyDriverProfile")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyDriverProfileQuery(), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("wallet")]
    [Authorize(Policy = Policies.DriverOnly)]
    [ProducesResponseType(typeof(DriverWalletDto), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves the current driver's wallet balance.")]
    [EndpointDescription("CanReceiveOrders is false when the balance is zero or below.")]
    [EndpointName("GetMyWallet")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetWallet(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyWalletQuery(), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("wallet/transactions")]
    [Authorize(Policy = Policies.DriverOnly)]
    [ProducesResponseType(typeof(PagedList<WalletTransactionDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves the driver's wallet transactions (paged).")]
    [EndpointDescription("Newest first. Optional type filter: TopUp, PlatformCommissionDeduction, CancellationCompensation.")]
    [EndpointName("GetMyWalletTransactions")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetWalletTransactions(
        [FromQuery] WalletTransactionType? type,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetMyWalletTransactionsQuery(type, pageNumber, pageSize), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("orders/active")]
    [Authorize(Policy = Policies.DriverOnly)]
    [ProducesResponseType(typeof(ActiveDeliveryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves the driver's current active order.")]
    [EndpointDescription("Returns 404 when the driver has no order in Preparing or OutForDelivery.")]
    [EndpointName("GetMyActiveOrder")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetActiveOrder(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyActiveOrderQuery(), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("orders/{serviceRequestId:guid}/release")]
    [Authorize(Policy = Policies.DriverOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Releases an accepted order.")]
    [EndpointDescription("Allowed only before pickup. The order is offered to other drivers.")]
    [EndpointName("ReleaseOrder")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Release(Guid serviceRequestId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ReleaseOrderCommand(serviceRequestId), cancellationToken);
        return result.Match(_ => NoContent(), Problem);
    }
}