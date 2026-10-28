using MediatR;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Addresses;
using UberSy.Domain.Common;
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
using UberSy.Infrastructure.Identity;

namespace UberSy.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
    , IDataProtectionKeyContext
{
    public DbSet<TbAddOn> TbAddOns => Set<TbAddOn>();

    public DbSet<TbAddress> TbAddresses => Set<TbAddress>();

    public DbSet<TbCategory> TbCategories => Set<TbCategory>();

    public DbSet<TbComplaint> TbComplaints => Set<TbComplaint>();

    public DbSet<TbComplaintMessage> TbComplaintMessages => Set<TbComplaintMessage>();

    public DbSet<TbDeliveryPricingRule> TbDeliveryPricingRules => Set<TbDeliveryPricingRule>();

    public DbSet<TbDiscount> TbDiscounts => Set<TbDiscount>();

    public DbSet<TbDiscountUsage> TbDiscountUsages => Set<TbDiscountUsage>();

    public DbSet<TbDriver> TbDrivers => Set<TbDriver>();

    public DbSet<TbDriverLocation> TbDriverLocations => Set<TbDriverLocation>();

    public DbSet<TbDriverWallet> TbDriverWallets => Set<TbDriverWallet>();

    public DbSet<TbDriverWalletTransaction> TbDriverWalletTransactions => Set<TbDriverWalletTransaction>();

    public DbSet<TbFoodOrder> TbFoodOrders => Set<TbFoodOrder>();

    public DbSet<TbItem> TbItems => Set<TbItem>();

    public DbSet<TbOrderItem> TbOrderItems => Set<TbOrderItem>();

    public DbSet<TbOrderItemAddOn> TbOrderItemAddOns => Set<TbOrderItemAddOn>();

    public DbSet<TbPayment> TbPayments => Set<TbPayment>();

    public DbSet<TbPrinter> TbPrinters => Set<TbPrinter>();

    public DbSet<TbRestaurant> TbRestaurants => Set<TbRestaurant>();

    public DbSet<TbRestaurantImage> TbRestaurantImages => Set<TbRestaurantImage>();

    public DbSet<TbReview> TbReviews => Set<TbReview>();

    public DbSet<TbRideRequest> TbRideRequests => Set<TbRideRequest>();

    public DbSet<TbServiceRequest> TbServiceRequests => Set<TbServiceRequest>();

    public DbSet<TbServiceRequestHistory> TbServiceRequestHistories => Set<TbServiceRequestHistory>();

    public DbSet<TbServiceRequestOffer> TbServiceRequestOffers => Set<TbServiceRequestOffer>();

    public DbSet<TbServiceZone> TbServiceZones => Set<TbServiceZone>();

    public DbSet<TbTableReservation> TbTableReservations => Set<TbTableReservation>();

    public DbSet<TbTag> TbTags => Set<TbTag>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    private readonly IMediator _mediator;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        //{
        //    var tableName = entityType.GetTableName();
        //    if (tableName is not null && !tableName.StartsWith("Tb"))
        //    {
        //        entityType.SetTableName("Tb" + tableName);
        //    }
        //}

        modelBuilder.UseCollation("Arabic_100_CI_AS");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var results = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEventsAsync(cancellationToken);
        return results;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = ChangeTracker.Entries()
                .Where(e => e.Entity is Entity baseEntity && baseEntity.DomainEvents.Count != 0)
                .Select(e => (Entity)e.Entity)
                .ToList();

        var domainEvents = domainEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (var entity in domainEntities)
        {
            entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}
