using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.Categories.Dtos;
using UberSy.Application.Features.Restaurants.Menu.Categories.Mappers;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants.Categories;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler(
    IApplicationDbContext context,
    IOwnershipValidator ownership,
    ILogger<CreateCategoryCommandHandler> logger) : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly ILogger<CreateCategoryCommandHandler> _logger = logger;

    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.TbRestaurants.AsNoTracking().FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (restaurant is null)
        {
            return ApplicationErrors.RestaurantNotFound;
        }
        var isOwner = await _ownership.EnsureIsOwnerByRestaurantIdAsync(restaurant.OwnerId, cancellationToken);
        if (isOwner.IsError)
            return isOwner.Errors;

        //if (request.PrinterId is not null)
        //{
        //    var printerBelongsToRestaurant = await _context.TbPrinters
        //        .AsNoTracking()
        //        .AnyAsync(p => p.Id == request.PrinterId && p.RestaurantId == request.RestaurantId, cancellationToken);

        //    if (!printerBelongsToRestaurant)
        //    {
        //        return ApplicationErrors.PrinterNotFound;
        //    }
        //}

        var createResult = TbCategory.Create(
            id: Guid.NewGuid(),
            restaurantId: request.RestaurantId,
            printerId: request.PrinterId,
            nameAr: request.NameAr,
            nameEn: request.NameEn,
            sortOrder: request.SortOrder);

        if (createResult.IsError)
        {
            _logger.LogError("Failed To Create Category: {Error}", createResult.TopError.Description);
            return createResult.Errors;
        }

        var category = createResult.Value;

        _context.TbCategories.Add(category);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category '{CategoryId}' Created For Restaurant '{RestaurantId}'.", category.Id, request.RestaurantId);

        return category.ToDto();
    }
}