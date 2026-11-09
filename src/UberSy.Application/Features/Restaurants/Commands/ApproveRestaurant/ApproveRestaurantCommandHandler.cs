using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.Restaurants.Commands.ApproveRestaurant;

public sealed class ApproveRestaurantCommandHandler(IApplicationDbContext context,IUserRoleService userRole , IUser user , ILogger<ApproveRestaurantCommandHandler> logger)
    : IRequestHandler<ApproveRestaurantCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUserRoleService _userRole = userRole;
    private readonly IUser _user = user;
    private readonly ILogger<ApproveRestaurantCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(ApproveRestaurantCommand request, CancellationToken cancellationToken)
    {
        if(_user.Id is null)
        {
            return ApplicationErrors.Unauthorized;
        }
        if (!_user.IsInRole(Roles.Admin))
        {
            return ApplicationErrors.AccessDenied;
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

