using NetTopologySuite.Geometries;
using UberSy.Domain.Drivers;
using Xunit;

namespace UberSy.Domain.UnitTests.Drivers;

public class DriverLocationTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static Point CreateValidPoint()
    {
        return new Point(36.2765, 33.5138) { SRID = 4326 };
    }

    private static TbDriverLocation CreateValidLocation()
    {
        return TbDriverLocation.Create(
            id: Guid.NewGuid(),
            driverId: Guid.NewGuid(),
            boundary: CreateValidPoint()
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenDriverIdIsEmpty()
    {
        var result = TbDriverLocation.Create(
            id: Guid.NewGuid(),
            driverId: Guid.Empty,
            boundary: CreateValidPoint());

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverErrors.UserIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenBoundaryIsNull()
    {
        var result = TbDriverLocation.Create(
            id: Guid.NewGuid(),
            driverId: Guid.NewGuid(),
            boundary: null!);

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverLocationErrors.BoundaryInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenBoundaryIsInvalid()
    {
        var invalidPoint = new Point(double.NaN, double.NaN) { SRID = 4326 };

        var result = TbDriverLocation.Create(
            id: Guid.NewGuid(),
            driverId: Guid.NewGuid(),
            boundary: invalidPoint);

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverLocationErrors.BoundaryInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var boundary = CreateValidPoint();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = TbDriverLocation.Create(id, driverId, boundary);

        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(driverId, result.Value.DriverId);
        Assert.Equal(boundary, result.Value.Boundary);

        // Verify UpdatedAt is set to UtcNow
        Assert.True(result.Value.UpdatedAt >= beforeCreation && result.Value.UpdatedAt <= afterCreation);
    }

    // ============================================================
    // Update Tests
    // ============================================================

    [Fact]
    public void UpdateLocation_ShouldReturnError_WhenBoundaryIsNull()
    {
        var location = CreateValidLocation();

        var result = location.UpdateLocation(null!);

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverLocationErrors.BoundaryInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateLocation_ShouldReturnError_WhenBoundaryIsInvalid()
    {
        var location = CreateValidLocation();
        var invalidPoint = new Point(double.NaN, double.NaN) { SRID = 4326 };

        var result = location.UpdateLocation(invalidPoint);

        Assert.False(result.IsSuccess);
        Assert.Equal(DriverLocationErrors.BoundaryInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateLocation_ShouldReturnSuccess_AndUpdateProperties()
    {
        // Arrange
        var location = CreateValidLocation();
        var originalUpdatedAt = location.UpdatedAt;

        var newPoint = new Point(36.2000, 33.5000) { SRID = 4326 };

        // Act
        var result = location.UpdateLocation(newPoint);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newPoint, location.Boundary);

        // Verify UpdatedAt has been refreshed
        Assert.True(location.UpdatedAt > originalUpdatedAt);
    }
}
