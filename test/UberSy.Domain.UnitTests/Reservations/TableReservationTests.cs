using UberSy.Domain.Reservations;
using Xunit;

namespace UberSy.Domain.UnitTests.Reservations;

public class TableReservationTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbTableReservation CreateValidReservation()
    {
        return TbTableReservation.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(),
            reservationTime: DateTime.UtcNow.AddDays(1), // Future date
            partySize: 4,
            depositAmount: 50m,
            note: "Window seat please"
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenRestaurantIdIsEmpty()
    {
        var result = TbTableReservation.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.Empty,
            reservationTime: DateTime.UtcNow.AddDays(1),
            partySize: 2,
            depositAmount: 50m,
            note: null);

        Assert.False(result.IsSuccess);
        Assert.Equal(TableReservationErrors.RestaurantIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenReservationTimeIsInThePast()
    {
        var result = TbTableReservation.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(),
            reservationTime: DateTime.UtcNow.AddDays(-1), // Past date
            partySize: 2,
            depositAmount: 50m,
            note: null);

        Assert.False(result.IsSuccess);
        Assert.Equal(TableReservationErrors.ReservationTimeMustBeFuture.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenReservationTimeIsNow()
    {
        var result = TbTableReservation.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(),
            reservationTime: DateTime.UtcNow, // Exact time
            partySize: 2,
            depositAmount: 50m,
            note: null);

        Assert.False(result.IsSuccess);
        Assert.Equal(TableReservationErrors.ReservationTimeMustBeFuture.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void Create_ShouldReturnError_WhenPartySizeIsZeroOrNegative(int invalidPartySize)
    {
        var result = TbTableReservation.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(),
            reservationTime: DateTime.UtcNow.AddDays(1),
            partySize: invalidPartySize,
            depositAmount: 50m,
            note: null);

        Assert.False(result.IsSuccess);
        Assert.Equal(TableReservationErrors.PartySizeZero.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenDepositAmountIsNegative()
    {
        var result = TbTableReservation.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(),
            reservationTime: DateTime.UtcNow.AddDays(1),
            partySize: 2,
            depositAmount: -10m,
            note: null);

        Assert.False(result.IsSuccess);
        Assert.Equal(TableReservationErrors.DepositAmountNegative.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenNoteIsTooLong()
    {
        var result = TbTableReservation.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(),
            reservationTime: DateTime.UtcNow.AddDays(1),
            partySize: 2,
            depositAmount: 50m,
            note: new string('A', 1001)); // 1001 characters

        Assert.False(result.IsSuccess);
        Assert.Equal(TableReservationErrors.NoteTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var reservationTime = DateTime.UtcNow.AddDays(2);
        var note = "  Birthday celebration  ";

        // Act
        var result = TbTableReservation.Create(
            id: id,
            restaurantId: restaurantId,
            reservationTime: reservationTime,
            partySize: 4,
            depositAmount: 0m, // Zero deposit is allowed
            note: note);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(restaurantId, result.Value.RestaurantId);
        Assert.Equal(reservationTime, result.Value.ReservationTime);
        Assert.Equal(4, result.Value.PartySize);
        Assert.Equal(0m, result.Value.DepositAmount);
        Assert.Equal("Birthday celebration", result.Value.Note); // Verify Trim()
    }

    // ============================================================
    // Reschedule Tests
    // ============================================================

    [Fact]
    public void Reschedule_ShouldReturnError_WhenNewTimeIsInThePast()
    {
        var reservation = CreateValidReservation();

        var result = reservation.Reschedule(
            reservationTime: DateTime.UtcNow.AddDays(-1), // Past date
            partySize: 2);

        Assert.False(result.IsSuccess);
        Assert.Equal(TableReservationErrors.ReservationTimeMustBeFuture.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Reschedule_ShouldReturnError_WhenNewPartySizeIsInvalid(int invalidPartySize)
    {
        var reservation = CreateValidReservation();

        var result = reservation.Reschedule(
            reservationTime: DateTime.UtcNow.AddDays(3),
            partySize: invalidPartySize);

        Assert.False(result.IsSuccess);
        Assert.Equal(TableReservationErrors.PartySizeZero.Code, result.TopError.Code);
    }

    [Fact]
    public void Reschedule_ShouldReturnSuccess_AndUpdateProperties()
    {
        // Arrange
        var reservation = CreateValidReservation();
        var newTime = DateTime.UtcNow.AddDays(5);
        var newPartySize = 6;

        // Act
        var result = reservation.Reschedule(newTime, newPartySize);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newTime, reservation.ReservationTime);
        Assert.Equal(newPartySize, reservation.PartySize);
    }
}
