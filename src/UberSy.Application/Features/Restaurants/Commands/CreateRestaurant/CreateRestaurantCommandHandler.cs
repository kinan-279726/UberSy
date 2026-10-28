using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Application.Features.Restaurants.Mappers;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants;

namespace UberSy.Application.Features.Restaurants.Commands.CreateRestaurant;

public sealed class CreateRestaurantCommandHandler(
    IApplicationDbContext context,
    IUser user,
    ILogger<CreateRestaurantCommandHandler> logger)
    : IRequestHandler<CreateRestaurantCommand, Result<RestaurantDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;
    private readonly ILogger<CreateRestaurantCommandHandler> _logger = logger;

    public async Task<Result<RestaurantDto>> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var boundary = new Point(request.Longitude, request.Latitude) { SRID = 4326 };

        if (_user.Id is null) return ApplicationErrors.Unauthorized;

        var createResult = TbRestaurant.Create(
           id: Guid.NewGuid(),
           ownerId: Guid.Parse(_user.Id),
           nameAr: request.NameAr,
           nameEn: request.NameEn,
           descriptionAr: request.DescriptionAr,
           descriptionEn: request.DescriptionEn,
           phone: request.Phone,
           email: request.Email,
           commissionRate: request.CommissionRate,
           taxRate: request.TaxRate,
           openingTime: request.OpeningTime,
           closingTime: request.ClosingTime,
           minOrderAmount: request.MinOrderAmount,
           maxOrderAmount: request.MaxOrderAmount,
           boundary: boundary);

        if (createResult.IsError)
        {
            _logger.LogWarning("Failed To Create Restaurant: {Error}", createResult.TopError.Description);
            return createResult.Errors;
        }

        var restaurant = createResult.Value;

        _context.TbRestaurants.Add(restaurant);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Restaurant '{RestaurantId}' Created Successfully.", restaurant.Id);

        return restaurant.ToDto();
    }
}