using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Constants;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.UpdateItemProfile;

public sealed class UpdateItemProfileCommandHandler(IApplicationDbContext context ,IOwnershipValidator ownership)
    : IRequestHandler<UpdateItemProfileCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(UpdateItemProfileCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.TbItems.FirstOrDefaultAsync(i => i.Id == request.ItemId , cancellationToken);
        if (item is null) return ApplicationErrors.ItemNotFound;

        var isOwner = await _ownership.EnsureIsOwnerByRestaurantIdAsync(item.RestaurantId , cancellationToken);
        if (isOwner.IsError)
            return isOwner.Errors;

        var updateResult = item.UpdateProfile(request.NameAr, request.NameEn, request.DescriptionAr, request.DescriptionEn, request.ImageUrl);
        if (updateResult.IsError) return updateResult.Errors;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}