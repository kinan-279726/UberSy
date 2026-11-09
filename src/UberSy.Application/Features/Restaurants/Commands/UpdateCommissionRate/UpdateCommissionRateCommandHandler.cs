using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateCommissionRate;

public sealed class UpdateCommissionRateCommandHandler(IApplicationDbContext context, IUser user) : IRequestHandler<UpdateCommissionRateCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;

    private readonly IUser _user = user;

    public async Task<Result<Updated>> Handle(UpdateCommissionRateCommand request, CancellationToken cancellationToken)
    {
        if (_user.Id is null)
        {
            return ApplicationErrors.Unauthorized;
        }
        if (!_user.IsInRole(Roles.Admin))
        {
            return ApplicationErrors.AccessDenied;
        }

        var restaurant = await _context.TbRestaurants.FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);
        if (restaurant is null) return ApplicationErrors.RestaurantNotFound;


        var result = restaurant.UpdateCommissionRate(request.CommissionRate);
        if (result.IsError) return result.Errors;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;

    }
}
