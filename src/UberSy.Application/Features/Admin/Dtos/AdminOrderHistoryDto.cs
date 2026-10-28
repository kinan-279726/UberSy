namespace UberSy.Application.Features.Admin.Dtos;

public sealed class AdminOrderHistoryDto
{
    public byte Status { get; init; }
    public string? Note { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
}
