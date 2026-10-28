using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers.Wallet;

namespace UberSy.Application.Features.Drivers.Commands.TopUpWallet;

public sealed record TopUpWalletCommand(decimal Amount , Guid DriverId) : IRequest<Result<Updated>>;

public sealed class TopUpWalletCommandHandler(IApplicationDbContext context,IUser user , ILogger<TopUpWalletCommandHandler> logger) : IRequestHandler<TopUpWalletCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;
    private readonly ILogger<TopUpWalletCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(TopUpWalletCommand request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Domain.Identity.Roles.Agent)) return ApplicationErrors.AccessDenied;

        var driver = await _context.TbDrivers.AnyAsync(d => d.Id == request.DriverId && d.IsApproved , cancellationToken);
        if (!driver) return ApplicationErrors.DriverNotFound;

        var wallet = await _context.TbDriverWallets.FirstOrDefaultAsync(w => w.DriverId == request.DriverId , cancellationToken);

        if (wallet is null) return ApplicationErrors.WalletNotFound;

        var walletResult = wallet.Deposit(request.Amount);

        if (walletResult.IsError) return walletResult.Errors;

        var transActionResult = TbDriverWalletTransaction.Create(
            id: Guid.NewGuid(),
            serviceRequestId : null,
            driverWalletId: wallet.Id,
            processedByUserId:Guid.Parse(_user.Id!),
            amount: request.Amount,
            type: Domain.Drivers.Wallet.Enums.WalletTransactionType.TopUp);

        if(transActionResult.IsError) return transActionResult.Errors;

        _context.TbDriverWalletTransactions.Add(transActionResult.Value);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("driver with id {driverId} deposit hes wallet by agent with id {userId}", request.DriverId, _user.Id);

        return Result.Updated;
    }
}