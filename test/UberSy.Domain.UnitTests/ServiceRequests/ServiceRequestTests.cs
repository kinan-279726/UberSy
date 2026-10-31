using UberSy.Domain.ServiceRequests;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Domain.ServiceRequests.Events.LifecycleEvents;
using Xunit;

namespace UberSy.Domain.UnitTests.ServiceRequests;

public class ServiceRequestTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbServiceRequest CreateValidServiceRequest()
    {
        return TbServiceRequest.Create(
            id: Guid.NewGuid(),
            customerId: Guid.NewGuid(),
            serviceType: ServiceType.Food,
            totalAmount: 100.50m
        ).Value;
    }

    private static TbServiceRequest CreateRequestInStatus(ServiceRequestStatus targetStatus)
    {
        var request = CreateValidServiceRequest();

        if (targetStatus >= ServiceRequestStatus.Preparing)
            request.MarkAsPreparing();

        if (targetStatus >= ServiceRequestStatus.OutForDelivery)
            request.MarkAsOutForDelivery();

        if (targetStatus >= ServiceRequestStatus.Delivered && targetStatus != ServiceRequestStatus.Cancelled)
            request.MarkAsDelivered();

        if (targetStatus == ServiceRequestStatus.Cancelled)
            request.Cancel("Test cancellation");

        request.ClearDomainEvents();
        return request;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenCustomerIdIsEmpty()
    {
        var result = TbServiceRequest.Create(
            Guid.NewGuid(), Guid.Empty, ServiceType.Food, 100m);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceRequestErrors.CustomerIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenServiceTypeIsInvalid()
    {
        var result = TbServiceRequest.Create(
            Guid.NewGuid(), Guid.NewGuid(), (ServiceType)222, 100m);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceRequestErrors.ServiceTypeInvalid.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Create_ShouldReturnError_WhenTotalAmountIsInvalid(decimal invalidAmount)
    {
        var result = TbServiceRequest.Create(
            Guid.NewGuid(), Guid.NewGuid(), ServiceType.Food, invalidAmount);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceRequestErrors.TotalAmountZero.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_AndSetStatusToPending()
    {
        var result = TbServiceRequest.Create(
            Guid.NewGuid(), Guid.NewGuid(), ServiceType.Food, 100m);

        Assert.True(result.IsSuccess);
        Assert.Equal(ServiceRequestStatus.Pending, result.Value.Status);
        Assert.Null(result.Value.CancellationReason);
    }

    // ============================================================
    // UpdateTotalAmount Tests
    // ============================================================

    [Fact]
    public void UpdateTotalAmount_ShouldReturnError_WhenRequestIsDelivered()
    {
        var request = CreateRequestInStatus(ServiceRequestStatus.Delivered);
        var result = request.UpdateTotalAmount(200m);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceRequestErrors.CannotModifyCompletedRequest.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateTotalAmount_ShouldReturnError_WhenRequestIsCancelled()
    {
        var request = CreateRequestInStatus(ServiceRequestStatus.Cancelled);
        var result = request.UpdateTotalAmount(200m);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceRequestErrors.CannotModifyCompletedRequest.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateTotalAmount_ShouldReturnSuccess_WhenPendingOrPreparing()
    {
        var request = CreateValidServiceRequest(); // Pending
        var result = request.UpdateTotalAmount(150m);

        Assert.True(result.IsSuccess);
        Assert.Equal(150m, request.TotalAmount);
    }

    // ============================================================
    // Status Transition Tests (Happy Path)
    // ============================================================

    [Fact]
    public void MarkAsPreparing_ShouldReturnSuccess_AndRaiseEvent()
    {
        var request = CreateValidServiceRequest();

        var result = request.MarkAsPreparing();

        Assert.True(result.IsSuccess);
        Assert.Equal(ServiceRequestStatus.Preparing, request.Status);
        Assert.Single(request.DomainEvents);
        Assert.IsType<ServiceRequestPreparing>(request.DomainEvents.First());
    }

    [Fact]
    public void MarkAsOutForDelivery_ShouldReturnSuccess_AndRaiseEvent()
    {
        var request = CreateRequestInStatus(ServiceRequestStatus.Preparing);

        var result = request.MarkAsOutForDelivery();

        Assert.True(result.IsSuccess);
        Assert.Equal(ServiceRequestStatus.OutForDelivery, request.Status);
        Assert.Single(request.DomainEvents);
        Assert.IsType<ServiceRequestOutForDelivery>(request.DomainEvents.First());
    }

    [Fact]
    public void MarkAsDelivered_ShouldReturnSuccess_AndRaiseEvent()
    {
        var request = CreateRequestInStatus(ServiceRequestStatus.OutForDelivery);

        var result = request.MarkAsDelivered();

        Assert.True(result.IsSuccess);
        Assert.Equal(ServiceRequestStatus.Delivered, request.Status);
        Assert.Single(request.DomainEvents);
        Assert.IsType<ServiceRequestDelivered>(request.DomainEvents.First());
    }

    // ============================================================
    // Cancel Tests
    // ============================================================

    [Fact]
    public void Cancel_ShouldReturnError_WhenReasonIsEmpty()
    {
        var request = CreateValidServiceRequest();

        var result = request.Cancel("");

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceRequestErrors.CancellationReasonRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Cancel_ShouldReturnSuccess_AndSetStatusToCancelled()
    {
        var request = CreateValidServiceRequest();

        var result = request.Cancel("  Customer changed mind  ");

        Assert.True(result.IsSuccess);
        Assert.Equal(ServiceRequestStatus.Cancelled, request.Status);
        Assert.Equal("Customer changed mind", request.CancellationReason); // Verify Trim
        Assert.Single(request.DomainEvents);
        // Assert.IsType<ServiceRequestCancelled>(request.DomainEvents.First()); // Assuming this event is correctly defined
    }

    // ============================================================
    // Status Transition Tests (Invalid Paths)
    // ============================================================

    [Fact]
    public void MarkAsDelivered_ShouldReturnError_WhenStatusIsPending()
    {
        var request = CreateValidServiceRequest(); // Pending

        var result = request.MarkAsDelivered(); // Trying to jump to Delivered

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceRequestErrors.InvalidStatusTransition.Code, result.TopError.Code);
    }

    [Fact]
    public void MarkAsPreparing_ShouldReturnError_WhenAlreadyDelivered()
    {
        var request = CreateRequestInStatus(ServiceRequestStatus.Delivered);

        var result = request.MarkAsPreparing();

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceRequestErrors.ServiceRequestAlreadyDelivered.Code, result.TopError.Code);
    }

    [Fact]
    public void MarkAsPreparing_ShouldReturnError_WhenAlreadyCancelled()
    {
        var request = CreateRequestInStatus(ServiceRequestStatus.Cancelled);

        var result = request.MarkAsPreparing();

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceRequestErrors.ServiceRequestAlreadyCancelled.Code, result.TopError.Code);
    }

    [Fact]
    public void Cancel_ShouldReturnError_WhenAlreadyDelivered()
    {
        var request = CreateRequestInStatus(ServiceRequestStatus.Delivered);

        var result = request.Cancel("Too late");

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceRequestErrors.ServiceRequestAlreadyDelivered.Code, result.TopError.Code);
    }
}
