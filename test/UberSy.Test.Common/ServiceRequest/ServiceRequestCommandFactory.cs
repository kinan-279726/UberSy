using UberSy.Application.Features.ServiceRequests.Commands.AcceptFoodOrder;
using UberSy.Application.Features.ServiceRequests.Commands.CancelService;
using UberSy.Application.Features.ServiceRequests.Commands.DriverCancelAfterAccepted;
using UberSy.Application.Features.ServiceRequests.Commands.MarkDelivered;
using UberSy.Application.Features.ServiceRequests.Commands.OutForDelivery;
using UberSy.Application.Features.ServiceRequests.Commands.PlaceFoodOrder;
using UberSy.Application.Features.ServiceRequests.Commands.RejectFoodOrder;
using UberSy.Application.Features.ServiceRequests.Offers.Commands.AcceptOffer;
using UberSy.Application.Features.ServiceRequests.Offers.Commands.RejectOffer;

namespace UberSy.Test.Common.ServiceRequest;

public static class ServiceRequestCommandFactory
{
    // 1. Accept Order (Restaurant)
    public static AcceptOrderCommand CreateAcceptOrderCommand(
        Guid? serviceRequestId = null)
    {
        return new AcceptOrderCommand(
            serviceRequestId ?? Guid.NewGuid());
    }

    // 2. Cancel Service
    public static CancelServiceCommand CreateCancelServiceCommand(
        Guid? serviceId = null)
    {
        return new CancelServiceCommand(
            serviceId ?? Guid.NewGuid());
    }

    // 3. Release Order
    public static ReleaseOrderCommand CreateReleaseOrderCommand(
        Guid? serviceRequestId = null)
    {
        return new ReleaseOrderCommand(
            serviceRequestId ?? Guid.NewGuid());
    }

    // 4. Mark Delivered
    public static MarkDeliveredCommand CreateMarkDeliveredCommand(
        Guid? serviceRequestId = null)
    {
        return new MarkDeliveredCommand(
            serviceRequestId ?? Guid.NewGuid());
    }

    // 5. Out For Delivery
    public static OutForDelivaryCommand CreateOutForDelivaryCommand(
        Guid? serviceRequestId = null)
    {
        return new OutForDelivaryCommand(
            serviceRequestId ?? Guid.NewGuid());
    }

    // 6. Place Food Order
    public static PlaceFoodOrderCommand CreatePlaceFoodOrderCommand(
        Guid? restaurantId = null,
        Guid? addressId = null,
        List<PlaceFoodOrderItem>? items = null)
    {
        return new PlaceFoodOrderCommand(
            restaurantId ?? Guid.NewGuid(),
            addressId ?? Guid.NewGuid(),
            items ?? [CreatePlaceFoodOrderItem()]);
    }

    // Helper to create PlaceFoodOrderItem
    public static PlaceFoodOrderItem CreatePlaceFoodOrderItem(
        Guid? itemId = null,
        int? quantity = null,
        List<Guid>? addOnIds = null)
    {
        return new PlaceFoodOrderItem(
            itemId ?? Guid.NewGuid(),
            quantity ?? 2,
            addOnIds ?? []);
    }

    // 7. Reject Order (Restaurant)
    public static RejectOrderCommand CreateRejectOrderCommand(
        Guid? serviceRequestId = null,
        string? reason = null)
    {
        return new RejectOrderCommand(
            serviceRequestId ?? Guid.NewGuid(),
            reason ?? "Restaurant is out of stock");
    }

    // 8. Accept Offer (Driver)
    public static AcceptOfferCommand CreateAcceptOfferCommand(
        Guid? offerId = null)
    {
        return new AcceptOfferCommand(
            offerId ?? Guid.NewGuid());
    }

    // 9. Reject Offer (Driver)
    public static RejectOfferCommand CreateRejectOfferCommand(
        Guid? offerId = null)
    {
        return new RejectOfferCommand(
            offerId ?? Guid.NewGuid());
    }
}
