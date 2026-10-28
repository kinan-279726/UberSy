namespace UberSy.Domain.Payments.Enums;

public enum PaymentStatus : byte
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
}