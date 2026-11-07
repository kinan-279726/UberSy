using UberSy.Application.Features.Drivers.Commands.TopUpWallet;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers.Wallet;

namespace UberSy.Test.Common.Driver;

public static class DriverWalletCommandFactory
{
    public static TopUpWalletCommand CreateTopUpWalletCommand(
        decimal? amount = null,
        Guid? driverId = null)
    {
        return new TopUpWalletCommand(
            amount ?? 100m,
            driverId ?? Guid.NewGuid());
    }
}
