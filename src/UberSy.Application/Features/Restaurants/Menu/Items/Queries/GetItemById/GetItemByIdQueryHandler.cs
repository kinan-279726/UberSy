namespace UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemById;

using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.Items.Dtos;
using UberSy.Application.Features.Restaurants.Menu.Items.Mappers;
using UberSy.Domain.Common.Results;

public sealed class GetItemByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetItemByIdQuery, Result<ItemDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<ItemDto>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.TbItems.AsNoTracking()
                                         .Include(a => a.AddOns.Where(x => x.IsActive == request.Active))
                                         .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);
        if (item is null)
        {
            return ApplicationErrors.ItemNotFound;
        }

        return item.ToDto();
    }
}