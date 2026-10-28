using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Addresses.Dtos;
using UberSy.Application.Features.Addresses.Mappers;
using UberSy.Domain.Common.Results;

public sealed class GetAddressByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAddressByIdQuery, Result<AddressDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<AddressDto>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        var address = await _context.TbAddresses.AsNoTracking().FirstOrDefaultAsync(a => a.Id == request.AddressId, cancellationToken);
        if (address is null) return ApplicationErrors.AddressNotFound;

        return address.ToDto();
    }
}