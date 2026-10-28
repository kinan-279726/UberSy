using Microsoft.EntityFrameworkCore;
using UberSy.Domain.Addresses;
using UberSy.Domain.Complaints;
using UberSy.Domain.DeliveryRule;
using UberSy.Domain.Discounts;
using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Wallet;
using UberSy.Domain.Identity;
using UberSy.Domain.Payments;
using UberSy.Domain.Reservations;
using UberSy.Domain.Restaurants;
using UberSy.Domain.Restaurants.AddOns;
using UberSy.Domain.Restaurants.Categories;
using UberSy.Domain.Restaurants.Images;
using UberSy.Domain.Restaurants.Items;
using UberSy.Domain.Restaurants.Printers;
using UberSy.Domain.Reviews;
using UberSy.Domain.ServiceRequests;
using UberSy.Domain.ServiceRequests.FoodOrders;
using UberSy.Domain.ServiceRequests.History;
using UberSy.Domain.ServiceRequests.Offers;
using UberSy.Domain.ServiceRequests.RideRequests;
using UberSy.Domain.ServiceZones;
using UberSy.Domain.Tags;

namespace UberSy.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<TbAddOn> TbAddOns { get; }

    public DbSet<TbAddress> TbAddresses { get; }

    public DbSet<TbCategory> TbCategories { get; }

    public DbSet<TbComplaint> TbComplaints { get; }

    public DbSet<TbComplaintMessage> TbComplaintMessages { get; }

    public DbSet<TbDeliveryPricingRule> TbDeliveryPricingRules { get; }

    public DbSet<TbDiscount> TbDiscounts {get;}

    public DbSet<TbDiscountUsage> TbDiscountUsages { get; }

    public DbSet<TbDriver> TbDrivers {get;}

    public DbSet<TbDriverLocation> TbDriverLocations {get;}

    public DbSet<TbDriverWallet> TbDriverWallets{get;}

    public DbSet<TbDriverWalletTransaction> TbDriverWalletTransactions {get;}

    public DbSet<TbFoodOrder> TbFoodOrders {get;}

    public DbSet<TbItem> TbItems {get;}

    public DbSet<TbOrderItem> TbOrderItems {get;}

    public DbSet<TbOrderItemAddOn> TbOrderItemAddOns {get;}

    public DbSet<TbPayment> TbPayments {get;}

    public DbSet<TbPrinter> TbPrinters {get;}
    public DbSet<TbRestaurant> TbRestaurants {get;}

    public DbSet<TbRestaurantImage> TbRestaurantImages {get;}

    public DbSet<TbReview> TbReviews {get;}

    public DbSet<TbRideRequest> TbRideRequests{get;}

    public DbSet<TbServiceRequest> TbServiceRequests {get;}

    public DbSet<TbServiceRequestHistory> TbServiceRequestHistories {get;}

    public DbSet<TbServiceRequestOffer> TbServiceRequestOffers{get;}

    public DbSet<TbServiceZone> TbServiceZones {get;}

    public DbSet<TbTableReservation> TbTableReservations{get;}

    public DbSet<TbTag> TbTags {get;}

    public DbSet<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
