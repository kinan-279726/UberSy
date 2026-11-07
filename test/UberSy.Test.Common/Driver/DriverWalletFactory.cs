using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers.Wallet;

namespace UberSy.Test.Common.Driver;

public static class DriverWalletFactory
{
    public static Result<TbDriverWallet> CreateDriverWallet(
        Guid? id = null,
        Guid? driverId = null)
    {
        return TbDriverWallet.Create(
            id ?? Guid.NewGuid(),
            driverId ?? Guid.NewGuid());
    }
}
