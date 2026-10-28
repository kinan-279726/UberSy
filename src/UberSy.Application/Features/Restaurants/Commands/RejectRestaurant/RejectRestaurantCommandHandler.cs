using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Constants;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.RejectRestaurant;

public sealed class RejectRestaurantCommandHandler(IApplicationDbContext context,IOwnershipValidator ownership, ILogger<RejectRestaurantCommandHandler> logger)
    : IRequestHandler<RejectRestaurantCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly ILogger<RejectRestaurantCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(RejectRestaurantCommand request, CancellationToken cancellationToken)
    {
        var isManager = _ownership.EnsureIsAManager(cancellationToken);
        if (isManager.IsError)
        {
            return isManager.Errors;
        }

        var restaurant = await _context.TbRestaurants.FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);
        if (restaurant is null) return ApplicationErrors.RestaurantNotFound;

        var result = restaurant.Reject();
        if (result.IsError) return result.Errors;

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Restaurant '{RestaurantId}' Rejected.", restaurant.Id);
        return Result.Updated;
    }
}