using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Restaurants.Commands.ApproveRestaurant;
using UberSy.Application.Features.Restaurants.Commands.RejectRestaurant;
using UberSy.Application.Features.Restaurants.Commands.UpdateCommissionRate;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Application.Features.Restaurants.Queries.GetPendingRestaurants;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers.Admin;

[Route("api/v{version:apiVersion}/admin/restaurants")]
[ApiVersion("1.0")]
[Authorize(Policy = Policies.AdminOnly)]
public sealed class AdminRestaurantsController(ISender sender) : ApiController
{
    [HttpGet("pending")]
    [ProducesResponseType(typeof(PagedList<RestaurantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Retrieves restaurants awaiting approval (paged).")]
    [EndpointDescription("Returns restaurants that are not approved yet, oldest first.")]
    [EndpointName("GetPendingRestaurants")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetPending(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPendingRestaurantsQuery(pageNumber, pageSize), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("{restaurantId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Approves a restaurant.")]
    [EndpointDescription("Approves the restaurant so customers can order from it.")]
    [EndpointName("ApproveRestaurant")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Approve(Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ApproveRestaurantCommand(restaurantId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{restaurantId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Rejects / suspends a restaurant.")]
    [EndpointDescription("Removes approval from the restaurant.")]
    [EndpointName("RejectRestaurant")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Reject(Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RejectRestaurantCommand(restaurantId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("{restaurantId:guid}/commission-rate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates the platform commission rate.")]
    [EndpointDescription("Sets the commission percentage the platform takes from this restaurant.")]
    [EndpointName("UpdateRestaurantCommissionRate")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateCommissionRate(Guid restaurantId,[FromBody] UpdateCommissionRateCommand request,CancellationToken cancellationToken)
    {
        var result = await sender.Send(request, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}