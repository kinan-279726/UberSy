using NetTopologySuite.Geometries;
using UberSy.Domain.Addresses;
using Xunit;

namespace UberSy.Domain.UnitTests.Addresses;

public class AddressTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static Point CreateValidPoint()
        => new Point(36.2765, 33.5138) { SRID = 4326 };

    private static TbAddress CreateValidAddress() =>
        TbAddress.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            street: "شارع المزة",
            city: "دمشق",
            boundary: CreateValidPoint(),
            label: "المنزل",
            floor: "الطابق الأول",
            phone: "+963956761655"
        ).Value;

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenUserIdIsEmpty()
    {
        var result = TbAddress.Create(
            id: Guid.NewGuid(),
            userId: Guid.Empty,
            street: "شارع المزة",
            city: "دمشق",
            boundary: CreateValidPoint(),
            label: "المنزل",
            floor: "1",
            phone: "+963956761655");

        Assert.False(result.IsSuccess);
        Assert.Equal(AddressErrors.UserIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenStreetIsEmpty()
    {
        var result = TbAddress.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            street: "",
            city: "دمشق",
            boundary: CreateValidPoint(),
            label: null,
            floor: null,
            phone: "+963956761655");

        Assert.False(result.IsSuccess);
        Assert.Equal(AddressErrors.StreetRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenStreetIsTooLong()
    {
        var result = TbAddress.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            street: new string('A', 101),
            city: "دمشق",
            boundary: CreateValidPoint(),
            label: null,
            floor: null,
            phone: "+963956761655");

        Assert.False(result.IsSuccess);
        Assert.Equal(AddressErrors.StreetTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCityIsEmpty()
    {
        var result = TbAddress.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            street: "شارع المزة",
            city: "",
            boundary: CreateValidPoint(),
            label: null,
            floor: null,
            phone: "+963956761655");

        Assert.False(result.IsSuccess);
        Assert.Equal(AddressErrors.CityRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCityIsTooLong()
    {
        var result = TbAddress.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            street: "شارع المزة",
            city: new string('B', 101),
            boundary: CreateValidPoint(),
            label: null,
            floor: null,
            phone: "+963956761655");

        Assert.False(result.IsSuccess);
        Assert.Equal(AddressErrors.CityTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenBoundaryIsNull()
    {
        var result = TbAddress.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            street: "شارع المزة",
            city: "دمشق",
            boundary: null!,
            label: null,
            floor: null,
            phone: "+963956761655");

        Assert.False(result.IsSuccess);
        Assert.Equal(AddressErrors.BoundaryRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenBoundaryIsInvalid()
    {
        var invalidPoint = new Point(double.NaN, double.NaN) { SRID = 4326 };

        var result = TbAddress.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            street: "شارع المزة",
            city: "دمشق",
            boundary: invalidPoint,
            label: null,
            floor: null,
            phone: "+963956761655");

        Assert.False(result.IsSuccess);
        Assert.Equal(AddressErrors.BoundaryInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenLabelIsTooLong()
    {
        var result = TbAddress.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            street: "شارع المزة",
            city: "دمشق",
            boundary: CreateValidPoint(),
            label: new string('C', 101),
            floor: null,
            phone: "+963956761655");

        Assert.False(result.IsSuccess);
        Assert.Equal(AddressErrors.LabelTooLong.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0999999999")]       
    [InlineData("+96399999999")]     
    [InlineData("+9639999999")]      
    [InlineData("+1234567890")]      
    public void Create_ShouldReturnError_WhenPhoneIsInvalid(string invalidPhone)
    {
        var result = TbAddress.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            street: "شارع المزة",
            city: "دمشق",
            boundary: CreateValidPoint(),
            label: null,
            floor: null,
            phone: invalidPhone);

        Assert.False(result.IsSuccess);
        Assert.Equal(AddressErrors.InvalidPhoneNumber.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenAllDataIsValid()
    {
        var result = TbAddress.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            street: "  شارع المزة  ",
            city: "  دمشق  ",
            boundary: CreateValidPoint(),
            label: " المنزل ",
            floor: " 1 ",
            phone: "+963956761655");

        Assert.True(result.IsSuccess);
        Assert.Equal("شارع المزة", result.Value.Street);
        Assert.Equal("دمشق", result.Value.City);
        Assert.Equal("المنزل", result.Value.Label);
    }

    // ============================================================
    // UpdateDetails Tests
    // ============================================================

    [Fact]
    public void UpdateDetails_ShouldReturnError_WhenDataIsInvalid()
    {
        var address = CreateValidAddress();

        var result = address.UpdateDetails(
            street: "",
            city: "دمشق",
            boundary: CreateValidPoint(),
            label: null,
            floor: null,
            phone: "+963956761655");

        Assert.False(result.IsSuccess);
        Assert.Equal(AddressErrors.StreetRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateDetails_ShouldReturnSuccess_AndUpdateFields()
    {
        var address = CreateValidAddress();
        var newPoint = new Point(36.2000, 33.5000) { SRID = 4326 };

        var result = address.UpdateDetails(
            street: "شارع بغداد",
            city: "حلب",
            boundary: newPoint,
            label: "العمل",
            floor: "الطابق الثالث",
            phone: "+963911111111");

        Assert.True(result.IsSuccess);
        Assert.Equal("شارع بغداد", address.Street);
        Assert.Equal("حلب", address.City);
        Assert.Equal(newPoint, address.Boundary);
        Assert.Equal("العمل", address.Label);
        Assert.Equal("الطابق الثالث", address.Floor);
        Assert.Equal("+963911111111", address.Phone);
    }

    // ============================================================
    // MarkAsDefault & UnmarkAsDefault Tests
    // ============================================================

    [Fact]
    public void MarkAsDefault_ShouldReturnSuccess_AndSetIsDefaultTrue()
    {
        // Arrange
        var address = CreateValidAddress();
        Assert.False(address.IsDefault);

        // Act
        var result = address.MarkAsDefault();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(address.IsDefault);
    }

    [Fact]
    public void UnmarkAsDefault_ShouldReturnSuccess_AndSetIsDefaultFalse()
    {
        var address = CreateValidAddress();
        address.MarkAsDefault();
        Assert.True(address.IsDefault);

        // Act
        var result = address.UnmarkAsDefault();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(address.IsDefault);
    }
}
