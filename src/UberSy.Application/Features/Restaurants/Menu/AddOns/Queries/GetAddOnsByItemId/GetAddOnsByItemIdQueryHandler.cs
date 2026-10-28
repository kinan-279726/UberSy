using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Mappers;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnsByItemId;

public sealed class GetAddOnsByItemIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAddOnsByItemIdQuery, Result<List<AddOnDto>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<List<AddOnDto>>> Handle(GetAddOnsByItemIdQuery request, CancellationToken cancellationToken)
    {
        var itemExists = await _context.TbItems.AsNoTracking().AnyAsync(i => i.Id == request.ItemId, cancellationToken);

        if (!itemExists)
        {
            return ApplicationErrors.ItemNotFound;
        }

        var addOns = await _context.TbItems
           .AsNoTracking()
           .Where(i => i.Id == request.ItemId)
           .SelectMany(i => i.AddOns)
           .Where(a => a.IsActive)
           .ToListAsync(cancellationToken);

        return addOns.ToDtos();
    }
}
