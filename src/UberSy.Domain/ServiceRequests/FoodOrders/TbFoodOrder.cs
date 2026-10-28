using UberSy.Domain.Addresses;
using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Discounts;
using UberSy.Domain.Drivers;
using UberSy.Domain.Restaurants;
using UberSy.Domain.ServiceRequests.Events.CreationEvents;
using UberSy.Domain.ServiceRequests.Events.DriverEvents;

namespace UberSy.Domain.ServiceRequests.FoodOrders;

public sealed class TbFoodOrder : AuditableEntity
{
    public Guid RestaurantId { get; private set; }
    public Guid AddressId { get; private set; }
    public Guid? DriverId { get; private set; }
    public Guid? DiscountId { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal DeliveryFee { get; private set; }
    public decimal DriverEarning { get; private set; }
    public decimal PlatformCommissionFromRestaurant { get; private set; }
    public DateTimeOffset ResponseTimeout { get; private set; }
    public TbAddress Address { get; private set; } = null!;
    public TbDiscount? Discount { get; private set; }
    public TbDriver? Driver { get; private set; }
    public TbServiceRequest IdNavigation { get; private set; } = null!;
    public TbRestaurant Restaurant { get; private set; } = null!;

    private readonly List<TbOrderItem> _orderItems = [];
    public IReadOnlyCollection<TbOrderItem> OrderItems => _orderItems.AsReadOnly();

    private TbFoodOrder()
    { }

    private TbFoodOrder(Guid id, Guid restaurantId, Guid addressId , DateTimeOffset responseTimeout)
        : base(id)
    {
        RestaurantId = restaurantId;
        AddressId = addressId;
        ResponseTimeout = responseTimeout;
        DiscountAmount = 0;
        DeliveryFee = 0;
        DriverEarning = 0;
        PlatformCommissionFromRestaurant = 0;
    }

    public static Result<TbFoodOrder> Create(Guid id, Guid restaurantId, Guid addressId , DateTimeOffset responseTimeout)
    {
        if (restaurantId == Guid.Empty) return FoodOrderErrors.RestaurantIdRequired;
        if (addressId == Guid.Empty) return FoodOrderErrors.AddressIdRequired;

        Console.WriteLine("UberSy.Domain.ServiceRequests.FoodOrders add this line to configurations");
        
        var order = new TbFoodOrder(id, restaurantId, addressId , responseTimeout);

        order.AddDomainEvent(new FoodOrderCreated(
                serviceRequestId: id,
                restaurantId: restaurantId,
                addressId: addressId,
                responseDeadlineUtc: responseTimeout));

        return order;
    }

    public Result<Updated> AssignDriver(Guid driverId)
    {
        if (DriverId is not null) return FoodOrderErrors.DriverAlreadyAssigned;
        if (driverId == Guid.Empty) return FoodOrderErrors.RestaurantIdRequired;

        DriverId = driverId;

        return Result.Updated;
    }

    public Result<Updated> RemoveDriver()
    {
        if (DriverId is null) return FoodOrderErrors.NoDriverAssigned;

        var previousDriverId = DriverId.Value;
        DriverId = null;
        
        AddDomainEvent(new DriverCancelledAfterAccept(Id, previousDriverId));

        return Result.Updated;
    }

    public Result<Updated> ApplyDiscount(Guid discountId, decimal discountAmount)
    {
        if (discountAmount < 0) return FoodOrderErrors.DiscountAmountNegative;

        DiscountId = discountId;
        DiscountAmount = discountAmount;

        return Result.Updated;
    }

    public Result<Updated> RecordDeliveryFinancials(decimal deliveryFee, decimal driverEarning, decimal platformCommission)
    {
        if (deliveryFee < 0) return FoodOrderErrors.DeliveryFeeNegative;
        if (driverEarning < 0) return FoodOrderErrors.DriverEarningNegative;
        if (driverEarning > deliveryFee) return FoodOrderErrors.DriverEarningExceedsDeliveryFee;
        if (platformCommission < 0) return FoodOrderErrors.CommissionNegative;

        DeliveryFee = deliveryFee;
        DriverEarning = driverEarning;
        PlatformCommissionFromRestaurant = platformCommission;

        return Result.Updated;
    }
}