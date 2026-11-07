using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.DeliveryRule;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Identity;
using UberSy.Test.Common.DeliveryPricingRule;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.DeliveryPricingRules.CreatePricingRule;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class CreatePricingRuleCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public CreatePricingRuleCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task AddExistingRuleAsync(
        bool isActive = true,
        VehicleType? vehicleType = null,
        TimeOnly? startHour = null,
        TimeOnly? endHour = null)
    {
        var result = DeliveryPricingRuleFactory.CreateDeliveryPricingRule(
            vehicleType: vehicleType,
            startHour: startHour,
            endHour: endHour);

        if (result.IsError)
        {
            throw new InvalidOperationException(
                $"DeliveryPricingRuleFactory produced invalid data: {result.TopError.Code} - {result.TopError.Description}");
        }

        var rule = result.Value;

        if (!isActive)
        {
            rule.Deactivate();
        }

        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);
    }

    private Task<int> CountRulesAsync()
        => _context.TbDeliveryPricingRules.AsNoTracking().CountAsync();

    [Fact]
    public async Task Handle_When_Admin_Creates_Valid_Rule_Should_Succeed_And_Persist()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            vehicleType: VehicleType.Motorcycle,
            serviceTier: ServiceTier.Standard,
            customerRatePerKm: 5m,
            driverRatePerKm: 3m,
            driverRatePerOrder: 1m,
            maxDistanceKm: 20m,
            startHour: new TimeOnly(8, 0),
            endHour: new TimeOnly(20, 0));

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var dto = result.Value;
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Equal(VehicleType.Motorcycle, dto.VehicleType);
        Assert.Equal(ServiceTier.Standard, dto.ServiceTier);
        Assert.Null(dto.CarType);
        Assert.Equal(5m, dto.CustomerRatePerKm);
        Assert.Equal(3m, dto.DriverRatePerKm);
        Assert.Equal(1m, dto.DriverRatePerOrder);
        Assert.Equal(20m, dto.MaxDistanceKm);
        Assert.Equal(new TimeOnly(8, 0), dto.StartHour);
        Assert.Equal(new TimeOnly(20, 0), dto.EndHour);
        Assert.True(dto.IsActive);

        var saved = await _context.TbDeliveryPricingRules.AsNoTracking().FirstAsync(r => r.Id == dto.Id);
        Assert.Equal(5m, saved.CustomerRatePerKm);
        Assert.True(saved.IsActive);
    }

    [Fact]
    public async Task Handle_When_Admin_Creates_Car_Rule_Should_Keep_CarType()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            vehicleType: VehicleType.Car,
            carType: CarType.Taxi);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(VehicleType.Car, result.Value.VehicleType);
        Assert.Equal(CarType.Taxi, result.Value.CarType);
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Admin_Should_Return_AccessDenied_And_Not_Create()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Customer);

        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand();

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
        Assert.Equal(0, await CountRulesAsync());
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Authenticated_Should_Return_AccessDenied()
    {
        // Arrange (no user set)
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand();

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_When_Schedule_Overlaps_Active_Rule_Should_Return_Overlap_Error()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        await AddExistingRuleAsync(startHour: new TimeOnly(8, 0), endHour: new TimeOnly(16, 0));

        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            startHour: new TimeOnly(12, 0),
            endHour: new TimeOnly(20, 0));

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.PricingRuleScheduleOverlap.Code, result.TopError.Code);
        Assert.Equal(1, await CountRulesAsync());
    }

    [Fact]
    public async Task Handle_When_Existing_All_Day_Rule_Overlaps_Should_Return_Overlap_Error()
    {
        // Arrange: an existing rule without hours covers the whole day
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        await AddExistingRuleAsync();

        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            startHour: new TimeOnly(8, 0),
            endHour: new TimeOnly(10, 0));

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.PricingRuleScheduleOverlap.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_When_Schedules_Only_Touch_At_The_Boundary_Should_Succeed()
    {
        // Arrange: existing 08:00-12:00, new 12:00-20:00 starts exactly at its end
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        await AddExistingRuleAsync(startHour: new TimeOnly(8, 0), endHour: new TimeOnly(12, 0));

        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            startHour: new TimeOnly(12, 0),
            endHour: new TimeOnly(20, 0));

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, await CountRulesAsync());
    }

    [Fact]
    public async Task Handle_When_Overlapping_Rule_Has_Different_VehicleType_Should_Succeed()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        await AddExistingRuleAsync(vehicleType: VehicleType.Bicycle);

        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(vehicleType: VehicleType.Motorcycle);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_When_Overlapping_Rule_Is_Inactive_Should_Succeed()
    {
        // Arrange: inactive rules are ignored by the overlap check
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        await AddExistingRuleAsync(isActive: false);

        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand();

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_With_Invalid_Command_Should_Return_Validation_Error_And_Not_Create()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(customerRatePerKm: -1m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "CustomerRatePerKm");
        Assert.Equal(0, await CountRulesAsync());
    }

    [Fact]
    public async Task Handle_When_Car_Vehicle_Has_No_CarType_Should_Return_Domain_Error()
    {
        // Arrange: passes the validator, rejected by the domain rules
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            vehicleType: VehicleType.Car,
            carType: null);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DeliveryRuleErrors.CarTypeRequired.Code, result.TopError.Code);
        Assert.Equal(0, await CountRulesAsync());
    }

    [Fact]
    public async Task Handle_When_Premium_Tier_Is_Used_With_Non_Car_Should_Return_Domain_Error()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            vehicleType: VehicleType.Motorcycle,
            serviceTier: ServiceTier.Premium);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DeliveryRuleErrors.ServiceTierRequiresCar.Code, result.TopError.Code);
    }
}
