using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateCommissionRate;

public sealed class UpdateCommissionRateCommandHandler(IApplicationDbContext context, IOwnershipValidator ownership) : IRequestHandler<UpdateCommissionRateCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(UpdateCommissionRateCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.TbRestaurants.FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);
        if (restaurant is null) return ApplicationErrors.RestaurantNotFound;

        var isManager = _ownership.EnsureIsAManager(cancellationToken);
        if (isManager.IsError)
            return isManager.Errors;

        var result = restaurant.UpdateCommissionRate(request.CommissionRate);
        if (result.IsError) return result.Errors;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;

    }
}