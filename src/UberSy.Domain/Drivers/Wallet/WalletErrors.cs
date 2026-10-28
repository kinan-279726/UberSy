using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Drivers.Wallet;

public static class WalletErrors
{
    public static Error AmountMustBePositive =>
        Error.Validation("Amount_Must_Be_Positive", "Transaction Amount Must Be Greater Than Zero");

    public static Error InsufficientBalance =>
        Error.Validation("Insufficient_Balance", "Wallet Balance Is Insufficient For This Deduction");

    public static Error TopUpRequiresProcessor =>
        Error.Validation("TopUp_Requires_Processor", "A TopUp Transaction Must Specify Who Processed It");

    public static Error DeductionRequiresServiceRequest =>
        Error.Validation("Deduction_Requires_Service_Request", "A Commission Deduction Must Reference A Service Request");

    public static Error WalletNotFound =>
        Error.NotFound("Wallet_Not_Found", "Driver Wallet Not Found");
}