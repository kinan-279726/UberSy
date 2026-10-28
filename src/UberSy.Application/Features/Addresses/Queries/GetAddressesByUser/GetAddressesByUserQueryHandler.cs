using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Addresses.Dtos;
using UberSy.Application.Features.Addresses.Mappers;
using UberSy.Domain.Common.Results;

public sealed class GetAddressesByUserQueryHandler(IApplicationDbContext context , IUser user)
    : IRequestHandler<GetAddressesByUserQuery, Result<List<AddressDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;

    public async Task<Result<List<AddressDto>>> Handle(GetAddressesByUserQuery request, CancellationToken cancellationToken)
    {
        if (_user.Id is null) return ApplicationErrors.Unauthorized;

        var addresses = await _context.TbAddresses
            .AsNoTracking()
            .Where(a => a.UserId == Guid.Parse(_user.Id) && !a.IsDeleted)
            .OrderByDescending(a => a.IsDefault)
            .ToListAsync(cancellationToken);

        return addresses.ToDtos();
    }
}