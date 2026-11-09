using UberSy.Application.Features.ServiceRequests.Queries.GetRestaurantOrders;
using UberSy.Domain.ServiceRequests.Enums;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.Queries.GetRestaurantOrders;

public sealed class GetRestaurantOrdersQueryValidatorTest
{
    private readonly GetRestaurantOrdersQueryValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(new GetRestaurantOrdersQuery(Guid.NewGuid(), ServiceRequestStatus.Pending, 1, 20)).IsValid);

    [Fact]
    public void Validate_ShouldPass_WhenStatusIsNull()
        => Assert.True(_validator.Validate(new GetRestaurantOrdersQuery(Guid.NewGuid(), null)).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenRestaurantIdEmpty()
    {
        var result = _validator.Validate(new GetRestaurantOrdersQuery(Guid.Empty, null));

        Assert.Contains(result.Errors, e => e.PropertyName == "RestaurantId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenStatusIsNotAnEnumValue()
    {
        var result = _validator.Validate(new GetRestaurantOrdersQuery(Guid.NewGuid(), (ServiceRequestStatus)99));

        Assert.Contains(result.Errors, e => e.PropertyName == "Status");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_ShouldFail_WhenPageNumberIsLessThanOne(int pageNumber)
    {
        var result = _validator.Validate(new GetRestaurantOrdersQuery(Guid.NewGuid(), null, pageNumber, 20));

        Assert.Contains(result.Errors, e => e.PropertyName == "PageNumber");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    public void Validate_ShouldFail_WhenPageSizeOutOfRange(int pageSize)
    {
        var result = _validator.Validate(new GetRestaurantOrdersQuery(Guid.NewGuid(), null, 1, pageSize));

        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    public void Validate_ShouldPass_WhenPageSizeOnBoundary(int pageSize)
        => Assert.True(_validator.Validate(new GetRestaurantOrdersQuery(Guid.NewGuid(), null, 1, pageSize)).IsValid);
}
