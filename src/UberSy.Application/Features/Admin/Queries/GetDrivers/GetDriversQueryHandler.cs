using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Admin.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.Admin.Queries.GetDrivers;

public sealed class GetDriversQueryHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<GetDriversQuery, Result<PagedList<AdminDriverListItemDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;
    //private readonly IDataProtector _protector = protector;

    public async Task<Result<PagedList<AdminDriverListItemDto>>> Handle(
        GetDriversQuery request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        var query = _context.TbDrivers.AsNoTracking().AsQueryable();

        if (request.IsApproved.HasValue)
            query = query.Where(d => d.IsApproved == request.IsApproved.Value);

        if (request.Status.HasValue)
            query = query.Where(d => d.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(d => d.NationalId.Contains(search)
                                     || (d.PlateNumber != null && d.PlateNumber.Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(d => d.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new AdminDriverListItemDto
            {
                Id = d.Id,
                UserId = d.UserId,
                VehicleType = d.VehicleType,
                ServiceTier = d.ServiceTier,
                CarType = d.CarType,
                NationalId = d.NationalId,
                PlateNumber = d.PlateNumber,
                Status = d.Status,
                IsApproved = d.IsApproved,
                WalletBalance = d.DriverWallet != null ? d.DriverWallet.Balance : 0m,
                CreatedAtUtc = d.CreatedAtUtc,
            })
            .ToListAsync(cancellationToken);

        return PagedList<AdminDriverListItemDto>.Create(items, request.PageNumber, request.PageSize, totalCount);
    }
}