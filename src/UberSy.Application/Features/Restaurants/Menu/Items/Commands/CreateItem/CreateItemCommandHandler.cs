using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.Items.Dtos;
using UberSy.Application.Features.Restaurants.Menu.Items.Mappers;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants.Items;

namespace UberSy.Application.Features.Restaurants.Menu.Items.requests.CreateItem;

public sealed class CreateItemCommandHandler(
    IApplicationDbContext context,
    IOwnershipValidator ownership,
    ILogger<CreateItemCommandHandler> logger)
    : IRequestHandler<CreateItemCommand, Result<ItemDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly ILogger<CreateItemCommandHandler> _logger = logger;

    public async Task<Result<ItemDto>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.TbRestaurants.AsNoTracking().FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);
        if (restaurant is null) return ApplicationErrors.RestaurantNotFound;

        var isOwner = await _ownership.EnsureIsOwnerByOwnerIdAsync(restaurant.OwnerId, cancellationToken);
        if (isOwner.IsError)
            return isOwner.Errors;

        var categoryBelongsToRestaurant = await _context.TbCategories
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.CategoryId && c.RestaurantId == request.RestaurantId, cancellationToken);

        if (!categoryBelongsToRestaurant)
        {
            return ApplicationErrors.CategoryNotFound;
        }

        var createResult = TbItem.Create(
            id: Guid.NewGuid(),
            restaurantId: request.RestaurantId,
            categoryId: request.CategoryId,
            nameAr: request.NameAr,
            nameEn: request.NameEn,
            descriptionAr: request.DescriptionAr,
            descriptionEn: request.DescriptionEn,
            price: request.Price,
            imageUrl: request.ImageUrl,
            calories: request.Calories,
            sortOrder: request.SortOrder);

        if (createResult.IsError)
        {
            _logger.LogError("Failed To Create Item: {Error}", createResult.TopError.Description);
            return createResult.Errors;
        }

        var item = createResult.Value;

        _context.TbItems.Add(item);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Item '{ItemId}' Created For Restaurant '{RestaurantId}'.", item.Id, request.RestaurantId);

        return item.ToDto();
    }
}