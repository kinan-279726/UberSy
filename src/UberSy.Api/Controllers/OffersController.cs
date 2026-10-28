using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Application.Features.Drivers.Queries.MyPendingOffers;
using UberSy.Application.Features.ServiceRequests.Offers.Commands.AcceptOffer;
using UberSy.Application.Features.ServiceRequests.Offers.Commands.RejectOffer;
using UberSy.Application.Features.ServiceRequests.Offers.Dtos;
using UberSy.Application.Features.ServiceRequests.Queries.GetPendingOffer;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers;

[Route("api/v{version:apiVersion}/offers")]
[ApiVersion("1.0")]
[Authorize(Policy = Policies.DriverOnly)]
public sealed class OffersController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<DriverOfferSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Retrieves the driver's active offers.")]
    [EndpointDescription("Offers still waiting for a response. Use after reconnecting to SignalR.")]
    [EndpointName("GetMyPendingOffers")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetMyPending(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyPendingOffersQuery(), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("{offerId:guid}")]
    [ProducesResponseType(typeof(PendingOfferDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Retrieves an offer's full details.")]
    [EndpointDescription("Locations, items, prices and the remaining time. 409 when the offer is no longer available.")]
    [EndpointName("GetOfferDetails")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetById(Guid offerId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPendingOfferQuery(offerId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("{offerId:guid}/accept")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Accepts an offer.")]
    [EndpointDescription("First driver wins. Others receive 409 (offer no longer available).")]
    [EndpointName("AcceptOffer")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Accept(Guid offerId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AcceptOfferCommand(offerId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{offerId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Rejects an offer.")]
    [EndpointDescription("The offer moves on to other drivers.")]
    [EndpointName("RejectOffer")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Reject(Guid offerId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RejectOfferCommand(offerId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}