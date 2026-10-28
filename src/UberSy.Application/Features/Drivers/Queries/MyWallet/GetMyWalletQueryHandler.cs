using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Queries.MyWallet;

public sealed class GetMyWalletQueryHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<GetMyWalletQuery, Result<DriverWalletDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<DriverWalletDto>> Handle(GetMyWalletQuery request, CancellationToken cancellationToken)
    {
        var driverResult = await _ownership.IsDriver();
        if (driverResult.IsError) return driverResult.Errors;

        var wallet = await _context.TbDriverWallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.DriverId == driverResult.Value, cancellationToken);

        if (wallet is null) return ApplicationErrors.WalletNotFound;

        return new DriverWalletDto
        {
            Balance = wallet.Balance,
            CanReceiveOrders = wallet.Balance > 0,
        };
    }
}
