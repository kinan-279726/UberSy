using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Queries.MyWalletTransactions;

public sealed class GetMyWalletTransactionsQueryHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<GetMyWalletTransactionsQuery, Result<PagedList<WalletTransactionDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<PagedList<WalletTransactionDto>>> Handle(
        GetMyWalletTransactionsQuery request, CancellationToken cancellationToken)
    {
        var driverResult = await _ownership.IsDriver();
        if (driverResult.IsError) return driverResult.Errors;
        var driverId = driverResult.Value;

        var query = _context.TbDriverWalletTransactions
            .AsNoTracking()
            .Where(t => t.DriverWallet.DriverId == driverId);

        if (request.Type.HasValue)
        {
            query = query.Where(t => t.Type == request.Type.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreatedAtUtc)
            .ThenByDescending(t => t.TransactionId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new WalletTransactionDto
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                Type = t.Type,
                ServiceRequestId = t.ServiceRequestId,
                TrackingNumber = t.ServiceRequest != null ? t.ServiceRequest.TrackingNumber : (int?)null,
                CreatedAtUtc = t.CreatedAtUtc,
            })
            .ToListAsync(cancellationToken);

        return PagedList<WalletTransactionDto>.Create(items, request.PageNumber, request.PageSize, totalCount);
    }
}