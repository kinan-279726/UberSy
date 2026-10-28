using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers.Wallet;
using UberSy.Domain.Drivers.Wallet.Enums;

namespace UberSy.Application.Features.ServiceRequests.Commands.MarkDelivered;

public sealed class MarkDeliveredCommandHandler(
    IApplicationDbContext context,
    IOwnershipValidator ownership,
    ILogger<MarkDeliveredCommandHandler> logger)
    : IRequestHandler<MarkDeliveredCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly ILogger<MarkDeliveredCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(MarkDeliveredCommand request, CancellationToken cancellation)
    {
        var driverResult = await _ownership.IsDriver();
        if (driverResult.IsError) return driverResult.Errors;
        var driverId = driverResult.Value;

        var foodOrder = await _context.TbFoodOrders
            .Include(o => o.IdNavigation)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceRequestId, cancellation);

        if (foodOrder is null) return ApplicationErrors.ServiceRequestNotFound;
        if (foodOrder.DriverId != driverId) return ApplicationErrors.AccessDenied;

        var result = foodOrder.IdNavigation.MarkAsDelivered();
        if (result.IsError) return result.Errors;

        var driver = await _context.TbDrivers.FirstAsync(d => d.Id == driverId, cancellation);
        var finishResult = driver.FinishDelivery();
        if (finishResult.IsError)
        {
            _logger.LogWarning("Could Not Reset Driver '{DriverId}' Status: {Error}", driverId, finishResult.TopError.Description);
        }

        var alreadyDeducted = await _context.TbDriverWalletTransactions
           .AnyAsync(t => t.ServiceRequestId == request.ServiceRequestId
                          && t.Type == WalletTransactionType.PlatformCommissionDeduction, cancellation);
        if (alreadyDeducted)
        {
            return ApplicationErrors.AlreadyDeducted;
        }

        var wallet = await _context.TbDriverWallets
            .FirstOrDefaultAsync(w => w.DriverId == foodOrder.DriverId, cancellation);

        if (wallet is null)
        {
            return ApplicationErrors.WalletNotFound;
        }

        var platformDue = foodOrder.PlatformCommissionFromRestaurant
                          + (foodOrder.DeliveryFee - foodOrder.DriverEarning);

        if (platformDue <= 0) return ApplicationErrors.CommissionNegative;

        var deduct = wallet.Deduct(platformDue);
        if (deduct.IsError)
        {
            _logger.LogError("Wallet Deduction Failed For Order '{Id}': {Error}", foodOrder.Id, deduct.TopError.Description);
            return deduct.Errors;
        }

        var transaction = TbDriverWalletTransaction.Create(
            id: Guid.NewGuid(),
            driverWalletId: wallet.Id,
            serviceRequestId: foodOrder.Id,
            processedByUserId: null,
            amount: -platformDue,
            type: WalletTransactionType.PlatformCommissionDeduction);

        if (transaction.IsError)
        {
            _logger.LogError("Wallet Transaction Failed For Order '{Id}': {Error}", foodOrder.Id, transaction.TopError.Description);
            return transaction.Errors;
        }
        _context.TbDriverWalletTransactions.Add(transaction.Value);

        await _context.SaveChangesAsync(cancellation);

        _logger.LogInformation("Order '{Id}' Delivered By Driver '{DriverId}'.", foodOrder.Id, driverId);
        _logger.LogInformation("Deducted {Amount} From Driver '{DriverId}' Wallet For Order '{Id}'.",
            platformDue, foodOrder.DriverId, foodOrder.Id);

        return Result.Updated;
    }
}