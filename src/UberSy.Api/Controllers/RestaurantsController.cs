using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Restaurants.Commands.CloseRestaurant;
using UberSy.Application.Features.Restaurants.Commands.CreateRestaurant;
using UberSy.Application.Features.Restaurants.Commands.OpenRestaurant;
using UberSy.Application.Features.Restaurants.Commands.UpdateOperatingHours;
using UberSy.Application.Features.Restaurants.Commands.UpdateOrderLimits;
using UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantImages;
using UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantLocation;
using UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantProfile;
using UberSy.Application.Features.Restaurants.Commands.UpdateTaxRate;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Application.Features.Restaurants.Queries.GetAllRestaurant;
using UberSy.Application.Features.Restaurants.Queries.GetRestaurantById;
using UberSy.Application.Features.Restaurants.Queries.GetRestaurantsByOwner;
using UberSy.Application.Features.Restaurants.Queries.SearchNearbyRestaurants;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Application.Features.ServiceRequests.Queries.GetFoodOrderDetails;
using UberSy.Application.Features.ServiceRequests.Queries.GetRestaurantOrders;
using UberSy.Application.Features.ServiceRequests.Queries.GetRestaurantStats;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers;

[Route("api/v{version:apiVersion}/restaurants")]
[ApiVersion("1.0")]
public sealed class RestaurantsController(ISender sender) : ApiController
{
    // ───────────── Customer / Public ─────────────

    [HttpGet("nearby")]
    [Authorize]
    [ProducesResponseType(typeof(List<RestaurantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Searches for restaurants near a location.")]
    [EndpointDescription("Returns approved restaurants within the given radius (km) ordered by distance.")]
    [EndpointName("SearchNearbyRestaurants")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> SearchNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new SearchNearbyRestaurantsQuery(latitude, longitude, radiusKm), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<RestaurantListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Retrieves approved restaurants (paged).")]
    [EndpointDescription("Search by Arabic or English name. Open restaurants come first, then by rating.")]
    [EndpointName("GetAllRestaurants")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAll(
    [FromQuery] string? search,
    [FromQuery] bool? isOpenNow,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetAllRestaurantQuery(search, isOpenNow, pageNumber, pageSize), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("{restaurantId:guid}", Name = "GetRestaurantById")]
    [Authorize]
    [ProducesResponseType(typeof(RestaurantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves a restaurant by id.")]
    [EndpointDescription("Returns the restaurant profile and current open/closed state.")]
    [EndpointName("GetRestaurantById")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetById(Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetRestaurantByIdQuery(restaurantId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    // ───────────── Owner ─────────────

    [HttpPost]
    //[Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(typeof(RestaurantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Creates a new restaurant.")]
    [EndpointDescription("Creates a restaurant owned by the current user. It stays unapproved until an admin approves it.")]
    [EndpointName("CreateRestaurant")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(request, cancellationToken);

        return result.Match(
            response => CreatedAtRoute(
                "GetRestaurantById",
                new { version = "1.0", restaurantId = response.Id },
                response),
            Problem);
    }

    [HttpGet("mine")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(typeof(List<RestaurantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Retrieves the current owner's restaurants.")]
    [EndpointDescription("Returns every restaurant owned by the authenticated user.")]
    [EndpointName("GetMyRestaurants")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetRestaurantsByOwnerQuery(), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPut("{restaurantId:guid}/profile")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates a restaurant profile.")]
    [EndpointDescription("Updates names, descriptions, phone and email of an owned restaurant.")]
    [EndpointName("UpdateRestaurantProfile")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateProfile(Guid restaurantId, [FromBody] UpdateRestaurantProfileCommand request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(request, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("{restaurantId:guid}/images")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates restaurant logo and cover images.")]
    [EndpointDescription("Sets the logo and cover image URLs of an owned restaurant.")]
    [EndpointName("UpdateRestaurantImages")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateImages(Guid restaurantId, [FromBody] UpdateRestaurantImagesCommand request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(request, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("{restaurantId:guid}/operating-hours")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates restaurant operating hours.")]
    [EndpointDescription("Sets the opening and closing time of an owned restaurant.")]
    [EndpointName("UpdateRestaurantOperatingHours")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateOperatingHours(Guid restaurantId, [FromBody] UpdateOperatingHoursCommand request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(request, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("{restaurantId:guid}/order-limits")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates minimum and maximum order amounts.")]
    [EndpointDescription("Sets the order amount limits of an owned restaurant.")]
    [EndpointName("UpdateRestaurantOrderLimits")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateOrderLimits(Guid restaurantId, [FromBody] UpdateOrderLimitsCommand request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(request, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("{restaurantId:guid}/location")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates the restaurant location.")]
    [EndpointDescription("Sets the latitude and longitude of an owned restaurant.")]
    [EndpointName("UpdateRestaurantLocation")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateLocation(Guid restaurantId, [FromBody] UpdateRestaurantLocationCommand request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(request, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("{restaurantId:guid}/tax-rate")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates the restaurant tax rate.")]
    [EndpointDescription("Sets the tax rate applied to orders of an owned restaurant.")]
    [EndpointName("UpdateRestaurantTaxRate")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateTaxRate(Guid restaurantId, [FromBody] UpdateTaxRateCommand request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(request,cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{restaurantId:guid}/open")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Opens the restaurant for orders.")]
    [EndpointDescription("Marks an approved, owned restaurant as open.")]
    [EndpointName("OpenRestaurant")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Open(Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new OpenRestaurantCommand(restaurantId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{restaurantId:guid}/close")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Closes the restaurant for orders.")]
    [EndpointDescription("Marks an owned restaurant as closed.")]
    [EndpointName("CloseRestaurant")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Close(Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CloseRestaurantCommand(restaurantId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpGet("{restaurantId:guid}/orders")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(typeof(PagedList<RestaurantOrderListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Retrieves the restaurant's orders (paged).")]
    [EndpointDescription("Optionally filter by status. Use status=Pending for orders waiting for the restaurant's response.")]
    [EndpointName("GetRestaurantOrders")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetOrders(
        Guid restaurantId,
        [FromQuery] ServiceRequestStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetRestaurantOrdersQuery(restaurantId, status, pageNumber, pageSize),
            cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("{restaurantId:guid}/orders/{serviceRequestId:guid}")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(typeof(FoodOrderDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves an order's details for the restaurant.")]
    [EndpointDescription("Items, add-ons and the restaurant's net amount only. No customer or driver data.")]
    [EndpointName("GetRestaurantOrderDetails")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetOrderDetails(Guid restaurantId, Guid serviceRequestId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetFoodOrderDetailsQuery(serviceRequestId), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("{restaurantId:guid}/stats")]
    [Authorize(Policy = Policies.OwnerOnly)]
    [ProducesResponseType(typeof(RestaurantStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Retrieves the restaurant's statistics.")]
    [EndpointDescription("Orders count, sales, commission, net earnings, daily breakdown and top items. Defaults to today (Damascus time).")]
    [EndpointName("GetRestaurantStats")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetStats(Guid restaurantId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetRestaurantStatsQuery(restaurantId, from, to), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }
}