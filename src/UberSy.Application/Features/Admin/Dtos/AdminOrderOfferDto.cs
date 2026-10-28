using UberSy.Domain.ServiceRequests.Offers.Enums;

namespace UberSy.Application.Features.Admin.Dtos;

public sealed class AdminOrderOfferDto
{
    public Guid OfferId { get; init; }
    public Guid DriverId { get; init; }
    public OfferStatus Status { get; init; }
    public DateTimeOffset SentAt { get; init; }
    public DateTimeOffset? RespondedAt { get; init; }
}