using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.ActivateCategory;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.CreateCategory;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.DeactivateCategory;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.RemovePrinterFromCategory;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategoryProfile;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategorySortOrder;
using UberSy.Application.Features.Restaurants.Menu.Categories.Dtos;
using UberSy.Application.Features.Restaurants.Menu.Categories.Quiries.GetCategoriesByRestaurant;
using UberSy.Application.Features.Restaurants.Menu.Categories.Quiries.GetCategoryById;
using UberSy.Contract.Requests.Menu;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers;

[Route("api/v{version:apiVersion}")]
[ApiVersion("1.0")]
public sealed class CategoriesController(ISender sender) : ApiController
{
    [HttpGet("restaurants/{restaurantId:guid}/categories")]
    [Authorize]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves the categories of a restaurant.")]
    [EndpointDescription("Returns the menu categories ordered by sort order.")]
    [EndpointName("GetCategoriesByRestaurant")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetByRestaurant(Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoriesByRestaurantQuery(restaurantId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("categories/{categoryId:guid}", Name = "GetCategoryById")]
    [Authorize]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves a category by id.")]
    [EndpointDescription("Returns a single menu category.")]
    [EndpointName("GetCategoryById")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetById(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(categoryId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("restaurants/{restaurantId:guid}/categories")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Creates a menu category.")]
    [EndpointDescription("Creates a category inside an owned restaurant.")]
    [EndpointName("CreateCategory")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create(
        Guid restaurantId,
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateCategoryCommand(restaurantId, request.PrinterId, request.NameAr, request.NameEn, request.SortOrder),
            cancellationToken);

        return result.Match(
            response => CreatedAtRoute("GetCategoryById", new { version = "1.0", categoryId = response.Id }, response),
            Problem);
    }

    [HttpPut("categories/{categoryId:guid}/profile")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates a category's names.")]
    [EndpointDescription("Updates the Arabic and English names of an owned category.")]
    [EndpointName("UpdateCategoryProfile")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateProfile(
        Guid categoryId,
        [FromBody] UpdateCategoryProfileRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateCategoryProfileCommand(categoryId, request.NameAr, request.NameEn), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("categories/{categoryId:guid}/sort-order")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates a category's sort order.")]
    [EndpointDescription("Changes the display position of an owned category.")]
    [EndpointName("UpdateCategorySortOrder")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateSortOrder(
        Guid categoryId,
        [FromBody] UpdateCategorySortOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateCategorySortOrderCommand(categoryId, request.SortOrder), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("categories/{categoryId:guid}/printer")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Assigns a printer to a category.")]
    [EndpointDescription("Orders of this category's items are printed on the assigned printer.")]
    [EndpointName("AssignPrinterToCategory")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> AssignPrinter(
        Guid categoryId,
        [FromBody] AssignPrinterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AssignPrinterToCategoryCommand(categoryId, request.PrinterId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpDelete("categories/{categoryId:guid}/printer")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Removes the printer from a category.")]
    [EndpointDescription("Detaches the assigned printer from an owned category.")]
    [EndpointName("RemovePrinterFromCategory")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> RemovePrinter(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemovePrinterFromCategoryCommand(categoryId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("categories/{categoryId:guid}/activate")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Activates a category.")]
    [EndpointDescription("Makes an owned category visible to customers.")]
    [EndpointName("ActivateCategory")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Activate(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateCategoryCommand(categoryId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("categories/{categoryId:guid}/deactivate")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Deactivates a category.")]
    [EndpointDescription("Hides an owned category from customers.")]
    [EndpointName("DeactivateCategory")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Deactivate(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateCategoryCommand(categoryId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}