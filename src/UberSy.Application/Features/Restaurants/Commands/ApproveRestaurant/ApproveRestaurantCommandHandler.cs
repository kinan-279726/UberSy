using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.Restaurants.Commands.ApproveRestaurant;

public sealed class ApproveRestaurantCommandHandler(IApplicationDbContext context,IUserRoleService userRole , IOwnershipValidator ownership , ILogger<ApproveRestaurantCommandHandler> logger)
    : IRequestHandler<ApproveRestaurantCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUserRoleService _userRole = userRole;
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly ILogger<ApproveRestaurantCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(ApproveRestaurantCommand request, CancellationToken cancellationToken)
    {
        var isManager =  _ownership.EnsureIsAManager(cancellationToken);
        if (isManager.IsError)
        {
            return isManager.Errors;
        }

        var restaurant = await _context.TbRestaurants.FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);
        if (restaurant is null) return ApplicationErrors.RestaurantNotFound;

        var result = restaurant.Approve();
        if (result.IsError) return result.Errors;

        var roleResult = await _userRole.AddToRoleAsync(restaurant.OwnerId, Roles.Owner);
        if(roleResult.IsError) return roleResult.Errors;

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Restaurant '{RestaurantId}' Approved.", restaurant.Id);
        return Result.Updated;
    }
}

