using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Api.Controllers;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.ActivateItem;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.AssignAddOnToItem;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.DeactivateItem;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.MoveItemToCategory;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.RemoveAddOnFromItem;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.UpdateItemPrice;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.UpdateItemProfile;
using UberSy.Application.Features.Restaurants.Menu.Items.Dtos;
using UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemById;
using UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemsByCategory;
using UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemsByRestaurant;
using UberSy.Application.Features.Restaurants.Menu.Items.requests.CreateItem;
using UberSy.Contract.Requests.Menu;
using UberSy.Infrastructure.Identity;

[Route("api/v{version:apiVersion}")]
[ApiVersion("1.0")]
public sealed class ItemsController(ISender sender) : ApiController
{
    [HttpGet("restaurants/{restaurantId:guid}/items")]
    [Authorize]
    [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves the items of a restaurant.")]
    [EndpointDescription("Returns all menu items of the restaurant.")]
    [EndpointName("GetItemsByRestaurant")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetByRestaurant(Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetItemsByRestaurantQuery(restaurantId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("categories/{categoryId:guid}/items")]
    [Authorize]
    [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves the items of a category.")]
    [EndpointDescription("Returns the menu items inside a category.")]
    [EndpointName("GetItemsByCategory")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetByCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetItemsByCategoryQuery(categoryId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("items/{itemId:guid}", Name = "GetItemById")]
    [Authorize]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves an item by id.")]
    [EndpointDescription("Returns a single menu item.")]
    [EndpointName("GetItemById")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetById(Guid itemId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetItemByIdQuery(itemId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("restaurants/{restaurantId:guid}/items")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Creates a menu item.")]
    [EndpointDescription("Creates an item inside a category of an owned restaurant.")]
    [EndpointName("CreateItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create(
        Guid restaurantId,
        [FromBody] CreateItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateItemCommand(
            restaurantId, request.CategoryId, request.NameAr, request.NameEn,
            request.DescriptionAr, request.DescriptionEn, request.Price,
            request.ImageUrl, request.Calories, request.SortOrder);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            response => CreatedAtRoute("GetItemById", new { version = "1.0", itemId = response.Id }, response),
            Problem);
    }

    [HttpPut("items/{itemId:guid}/profile")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates an item's profile.")]
    [EndpointDescription("Updates names, descriptions and image of an owned item.")]
    [EndpointName("UpdateItemProfile")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateProfile(
        Guid itemId,
        [FromBody] UpdateItemProfileRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateItemProfileCommand(itemId, request.NameAr, request.NameEn,
                request.DescriptionAr, request.DescriptionEn, request.ImageUrl),
            cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("items/{itemId:guid}/price")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates an item's price.")]
    [EndpointDescription("Changes the price of an owned item. Existing orders keep their frozen price.")]
    [EndpointName("UpdateItemPrice")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdatePrice(
        Guid itemId,
        [FromBody] UpdateItemPriceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateItemPriceCommand(itemId, request.Price), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("items/{itemId:guid}/category")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Moves an item to another category.")]
    [EndpointDescription("The new category must belong to the same restaurant.")]
    [EndpointName("MoveItemToCategory")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> MoveToCategory(
        Guid itemId,
        [FromBody] MoveItemToCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new MoveItemToCategoryCommand(itemId, request.NewCategoryId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("items/{itemId:guid}/activate")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Activates an item.")]
    [EndpointDescription("Makes an owned item available for ordering.")]
    [EndpointName("ActivateItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Activate(Guid itemId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateItemCommand(itemId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("items/{itemId:guid}/deactivate")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Deactivates an item.")]
    [EndpointDescription("Makes an owned item unavailable (e.g. sold out).")]
    [EndpointName("DeactivateItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Deactivate(Guid itemId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateItemCommand(itemId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("items/{itemId:guid}/addons/{addOnId:guid}")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Assigns an add-on to an item.")]
    [EndpointDescription("Makes the add-on selectable for the item.")]
    [EndpointName("AssignAddOnToItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> AssignAddOn(Guid itemId, Guid addOnId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AssignAddOnToItemCommand(itemId, addOnId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpDelete("items/{itemId:guid}/addons/{addOnId:guid}")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Removes an add-on from an item.")]
    [EndpointDescription("The add-on is no longer selectable for the item.")]
    [EndpointName("RemoveAddOnFromItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> RemoveAddOn(Guid itemId, Guid addOnId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveAddOnFromItemCommand(itemId, addOnId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}