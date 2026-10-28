using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Mappers;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnById;

public sealed class GetAddOnByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAddOnByIdQuery, Result<AddOnDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<AddOnDto>> Handle(GetAddOnByIdQuery request, CancellationToken cancellationToken)
    {
        var addOn = await _context.TbAddOns.AsNoTracking().FirstOrDefaultAsync(a => a.Id == request.AddOnId, cancellationToken);

        if (addOn is null) return ApplicationErrors.ItemNotFound;

        return addOn.ToDto();
    }
}