using UberSy.Domain.ServiceRequests.Events;
using UberSy.Domain.ServiceRequests.Events.DriverEvents;
using UberSy.Domain.ServiceRequests.Offers;
using UberSy.Domain.ServiceRequests.Offers.Enums;
using Xunit;

namespace UberSy.Domain.UnitTests.ServiceRequests.Offers;

public class ServiceRequestOfferTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbServiceRequestOffer CreateValidOffer()
    {
        return TbServiceRequestOffer.Create(
            id: Guid.NewGuid(),
            serviceRequestId: Guid.NewGuid(),
            driverId: Guid.NewGuid(),
            responseTimeout: DateTimeOffset.UtcNow.AddMinutes(1)
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenDriverIdIsEmpty()
    {
        var result = TbServiceRequestOffer.Create(
            id: Guid.NewGuid(),
            serviceRequestId: Guid.NewGuid(),
            driverId: Guid.Empty,
            responseTimeout: DateTimeOffset.UtcNow.AddMinutes(1));

        Assert.False(result.IsSuccess);
        Assert.Equal(OfferErrors.DriverIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_AndRaiseDomainEvent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var serviceRequestId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var timeout = DateTimeOffset.UtcNow.AddMinutes(1);

        // Act
        var result = TbServiceRequestOffer.Create(id, serviceRequestId, driverId, timeout);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OfferStatus.Sent, result.Value.Status);
        Assert.Equal(timeout, result.Value.ResponseTimeout);
        Assert.Null(result.Value.RespondedAt);

        // Verify Domain Event
        Assert.Single(result.Value.DomainEvents);
        Assert.IsType<ServiceRequestOfferSent>(result.Value.DomainEvents.First());
    }

    // ============================================================
    // Accept Tests
    // ============================================================

    [Fact]
    public void Accept_ShouldReturnError_WhenStatusIsNotSent()
    {
        var offer = CreateValidOffer();
        offer.Accept(); // Set to Accepted

        var result = offer.Accept(); // Try again

        Assert.False(result.IsSuccess);
        Assert.Equal(OfferErrors.OfferAlreadyRespondedTo.Code, result.TopError.Code);
    }

    [Fact]
    public void Accept_ShouldReturnSuccess_AndRaiseDomainEvent()
    {
        var offer = CreateValidOffer();
        offer.ClearDomainEvents(); // Clear creation event for testing

        var result = offer.Accept();

        Assert.True(result.IsSuccess);
        Assert.Equal(OfferStatus.Accepted, offer.Status);
        Assert.NotNull(offer.RespondedAt);

        // Verify Domain Event
        Assert.Single(offer.DomainEvents);
        Assert.IsType<DriverAcceptedOffer>(offer.DomainEvents.First());
    }

    // ============================================================
    // Reject Tests
    // ============================================================

    [Fact]
    public void Reject_ShouldReturnError_WhenStatusIsNotSent()
    {
        var offer = CreateValidOffer();
        offer.Reject(); // Set to Rejected

        var result = offer.Reject(); // Try again

        Assert.False(result.IsSuccess);
        Assert.Equal(OfferErrors.OfferAlreadyRespondedTo.Code, result.TopError.Code);
    }

    [Fact]
    public void Reject_ShouldReturnSuccess_AndRaiseDomainEvent()
    {
        var offer = CreateValidOffer();
        offer.ClearDomainEvents();

        var result = offer.Reject();

        Assert.True(result.IsSuccess);
        Assert.Equal(OfferStatus.Rejected, offer.Status);
        Assert.NotNull(offer.RespondedAt);

        // Verify Domain Event
        Assert.Single(offer.DomainEvents);
        Assert.IsType<DriverRejectedOffer>(offer.DomainEvents.First());
    }

    // ============================================================
    // Expire Tests
    // ============================================================

    [Fact]
    public void Expire_ShouldReturnError_WhenStatusIsNotSent()
    {
        var offer = CreateValidOffer();
        offer.Expire(); // Set to Expired

        var result = offer.Expire(); // Try again

        Assert.False(result.IsSuccess);
        Assert.Equal(OfferErrors.OfferAlreadyRespondedTo.Code, result.TopError.Code);
    }

    [Fact]
    public void Expire_ShouldReturnSuccess_AndRaiseDomainEvent()
    {
        var offer = CreateValidOffer();
        offer.ClearDomainEvents();

        var result = offer.Expire();

        Assert.True(result.IsSuccess);
        Assert.Equal(OfferStatus.Expired, offer.Status);
        Assert.Null(offer.RespondedAt); // RespondedAt is not set on Expire

        // Verify Domain Event
        Assert.Single(offer.DomainEvents);
        Assert.IsType<DriverOfferExpired>(offer.DomainEvents.First());
    }

    // ============================================================
    // MarkAsLostToAnotherDriver Tests
    // ============================================================

    [Fact]
    public void MarkAsLostToAnotherDriver_ShouldReturnSuccess_WhenStatusIsSent()
    {
        var offer = CreateValidOffer();
        Assert.Equal(OfferStatus.Sent, offer.Status);

        var result = offer.MarkAsLostToAnotherDriver();

        Assert.True(result.IsSuccess);
        Assert.Equal(OfferStatus.LostToAnotherDriver, offer.Status);
    }

    [Fact]
    public void MarkAsLostToAnotherDriver_ShouldDoNothing_WhenStatusIsNotSent()
    {
        var offer = CreateValidOffer();
        offer.Accept(); // Status is now Accepted, not Sent
        Assert.Equal(OfferStatus.Accepted, offer.Status);

        var result = offer.MarkAsLostToAnotherDriver();

        Assert.True(result.IsSuccess); // It returns Updated even though no change happened
        Assert.Equal(OfferStatus.Accepted, offer.Status); // Status remains Accepted
    }
}
