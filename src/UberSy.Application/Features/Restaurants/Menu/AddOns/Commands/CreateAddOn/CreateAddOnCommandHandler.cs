using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Mappers;
using UberSy.Domain.Common.Constants;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants.AddOns;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.CreateAddOn;

public sealed class CreateAddOnCommandHandler(
    IApplicationDbContext context,
    IOwnershipValidator ownership,
    ILogger<CreateAddOnCommandHandler> logger)
    : IRequestHandler<CreateAddOnCommand, Result<AddOnDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly ILogger<CreateAddOnCommandHandler> _logger = logger;

    public async Task<Result<AddOnDto>> Handle(CreateAddOnCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.TbRestaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (restaurant is null)
        {
            return ApplicationErrors.RestaurantNotFound;
        }

        var isOwner = await _ownership.EnsureIsOwnerByOwnerIdAsync(restaurant.OwnerId, cancellationToken);
        if (isOwner.IsError)
        {
            return isOwner.Errors;
        }

        var createResult = TbAddOn.Create(
           id: Guid.NewGuid(),
           restaurantId: request.RestaurantId,
           nameAr: request.NameAr,
           nameEn: request.NameEn,
           price: request.Price);

        if (createResult.IsError)
        {
            _logger.LogWarning("Failed To Create AddOn: {Error}", createResult.TopError.Description);
            return createResult.Errors;
        }

        var addOn = createResult.Value;

        _context.TbAddOns.Add(addOn);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("AddOn '{AddOnId}' Created For Restaurant '{RestaurantId}'.", addOn.Id, request.RestaurantId);

        return addOn.ToDto();


    }
}