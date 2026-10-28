namespace UberSy.Application.Features.Drivers.Dtos;

public sealed class DriverWalletDto
{
    public decimal Balance { get; init; }
    public bool CanReceiveOrders { get; init; }
}