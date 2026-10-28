using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.ActivateAddOn;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.CreateAddOn;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.DeactivateAddOn;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.UpdateAddOn;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnById;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnsByItemId;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnsByRestaurant;
using UberSy.Contract.Requests.Menu;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers;

[Route("api/v{version:apiVersion}")]
[ApiVersion("1.0")]
public sealed class AddOnsController(ISender sender) : ApiController
{
    [HttpGet("restaurants/{restaurantId:guid}/addons")]
    [Authorize]
    [ProducesResponseType(typeof(List<AddOnDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves the add-ons of a restaurant.")]
    [EndpointDescription("Returns all add-ons defined by the restaurant.")]
    [EndpointName("GetAddOnsByRestaurant")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetByRestaurant(Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAddOnsByRestaurantQuery(restaurantId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("items/{itemId:guid}/addons")]
    [Authorize]
    [ProducesResponseType(typeof(List<AddOnDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves the add-ons of an item.")]
    [EndpointDescription("Returns the add-ons a customer can choose for the item.")]
    [EndpointName("GetAddOnsByItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetByItem(Guid itemId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAddOnsByItemIdQuery(itemId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("addons/{addOnId:guid}", Name = "GetAddOnById")]
    [Authorize]
    [ProducesResponseType(typeof(AddOnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves an add-on by id.")]
    [EndpointDescription("Returns a single add-on.")]
    [EndpointName("GetAddOnById")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetById(Guid addOnId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAddOnByIdQuery(addOnId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("restaurants/{restaurantId:guid}/addons")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(typeof(AddOnDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Creates an add-on.")]
    [EndpointDescription("Creates an add-on for an owned restaurant.")]
    [EndpointName("CreateAddOn")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create(
        Guid restaurantId,
        [FromBody] CreateAddOnRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateAddOnCommand(restaurantId, request.NameAr, request.NameEn, request.Price),
            cancellationToken);

        return result.Match(
            response => CreatedAtRoute("GetAddOnById", new { version = "1.0", addOnId = response.Id }, response),
            Problem);
    }

    [HttpPut("addons/{addOnId:guid}")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates an add-on.")]
    [EndpointDescription("Updates names and price of an owned add-on.")]
    [EndpointName("UpdateAddOn")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(
        Guid addOnId,
        [FromBody] UpdateAddOnRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateAddOnCommand(addOnId, request.NameAr, request.NameEn, request.Price),
            cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("addons/{addOnId:guid}/activate")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Activates an add-on.")]
    [EndpointDescription("Makes an owned add-on selectable.")]
    [EndpointName("ActivateAddOn")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Activate(Guid addOnId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateAddOnCommand(addOnId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("addons/{addOnId:guid}/deactivate")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Deactivates an add-on.")]
    [EndpointDescription("Makes an owned add-on unavailable.")]
    [EndpointName("DeactivateAddOn")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Deactivate(Guid addOnId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateAddOnCommand(addOnId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}