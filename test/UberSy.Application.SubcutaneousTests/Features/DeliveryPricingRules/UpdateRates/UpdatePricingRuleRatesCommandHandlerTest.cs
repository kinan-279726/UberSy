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

namespace UberSy.Application.SubcutaneousTests.Features.DeliveryPricingRules.UpdateRates;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdatePricingRuleRatesCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public UpdatePricingRuleRatesCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    // Creates a rule with customer rate 5, driver rate 3 and no per-order rate unless specified
    private async Task<TbDeliveryPricingRule> AddRuleAsync(
        VehicleType? vehicleType = null,
        decimal? driverRatePerOrder = null)
    {
        var result = DeliveryPricingRuleFactory.CreateDeliveryPricingRule(
            vehicleType: vehicleType,
            customerRatePerKm: 5m,
            driverRatePerKm: 3m,
            driverRatePerOrder: driverRatePerOrder);

        if (result.IsError)
        {
            throw new InvalidOperationException(
                $"DeliveryPricingRuleFactory produced invalid data: {result.TopError.Code} - {result.TopError.Description}");
        }

        var rule = result.Value;
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        return rule;
    }

    private Task<TbDeliveryPricingRule> GetRuleFromDbAsync(Guid ruleId)
        => _context.TbDeliveryPricingRules.AsNoTracking().FirstAsync(r => r.Id == ruleId);

    private async Task AssertRatesUnchangedAsync(Guid ruleId, decimal? expectedDriverRatePerOrder = null)
    {
        var rule = await GetRuleFromDbAsync(ruleId);

        Assert.Equal(5m, rule.CustomerRatePerKm);
        Assert.Equal(3m, rule.DriverRatePerKm);
        Assert.Equal(expectedDriverRatePerOrder, rule.DriverRatePerOrder);
    }

    [Fact]
    public async Task Handle_When_Admin_Updates_Rates_Should_Succeed_And_Persist()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync();

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleRatesCommand(
            rule.Id,
            customerRatePerKm: 8m,
            driverRatePerKm: 6m,
            driverRatePerOrder: 1.5m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var updated = await GetRuleFromDbAsync(rule.Id);
        Assert.Equal(8m, updated.CustomerRatePerKm);
        Assert.Equal(6m, updated.DriverRatePerKm);
        Assert.Equal(1.5m, updated.DriverRatePerOrder);
    }

    [Fact]
    public async Task Handle_When_DriverRatePerOrder_Is_Null_Should_Clear_The_Existing_Value()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync(driverRatePerOrder: 2m);

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleRatesCommand(
            rule.Id,
            customerRatePerKm: 5m,
            driverRatePerKm: 3m,
            driverRatePerOrder: null);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null((await GetRuleFromDbAsync(rule.Id)).DriverRatePerOrder);
    }

    [Fact]
    public async Task Handle_When_DriverRate_Equals_CustomerRate_Should_Succeed()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync();

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleRatesCommand(
            rule.Id,
            customerRatePerKm: 6m,
            driverRatePerKm: 6m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_Should_Only_Update_The_Requested_Rule()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var target = await AddRuleAsync(VehicleType.Motorcycle);
        var other = await AddRuleAsync(VehicleType.Bicycle);

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleRatesCommand(
            target.Id,
            customerRatePerKm: 9m,
            driverRatePerKm: 7m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(9m, (await GetRuleFromDbAsync(target.Id)).CustomerRatePerKm);
        await AssertRatesUnchangedAsync(other.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_When_CustomerRate_Is_Not_Positive_Should_Return_Domain_Error_And_Not_Update(double value)
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync();

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleRatesCommand(
            rule.Id,
            customerRatePerKm: (decimal)value,
            driverRatePerKm: 3m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DeliveryRuleErrors.CustomerRatePerKmInvalid.Code, result.TopError.Code);
        await AssertRatesUnchangedAsync(rule.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_When_DriverRate_Is_Not_Positive_Should_Return_Domain_Error_And_Not_Update(double value)
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync();

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleRatesCommand(
            rule.Id,
            customerRatePerKm: 5m,
            driverRatePerKm: (decimal)value);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DeliveryRuleErrors.DriverRatePerKmInvalid.Code, result.TopError.Code);
        await AssertRatesUnchangedAsync(rule.Id);
    }

    [Fact]
    public async Task Handle_When_DriverRate_Exceeds_CustomerRate_Should_Return_Domain_Error_And_Not_Update()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync();

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleRatesCommand(
            rule.Id,
            customerRatePerKm: 5m,
            driverRatePerKm: 6m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DeliveryRuleErrors.DriverRatePerKmExceedsCustomer.Code, result.TopError.Code);
        await AssertRatesUnchangedAsync(rule.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_When_DriverRatePerOrder_Is_Not_Positive_Should_Return_Domain_Error_And_Not_Update(double value)
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync();

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleRatesCommand(
            rule.Id,
            customerRatePerKm: 5m,
            driverRatePerKm: 3m,
            driverRatePerOrder: (decimal)value);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DeliveryRuleErrors.DriverRatePerOrderInvalid.Code, result.TopError.Code);
        await AssertRatesUnchangedAsync(rule.Id);
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Admin_Should_Return_AccessDenied_And_Not_Update()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Customer);

        var rule = await AddRuleAsync();

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleRatesCommand(
            rule.Id,
            customerRatePerKm: 8m,
            driverRatePerKm: 6m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
        await AssertRatesUnchangedAsync(rule.Id);
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Authenticated_Should_Return_AccessDenied()
    {
        // Arrange (no user set)
        var rule = await AddRuleAsync();

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleRatesCommand(
            rule.Id,
            customerRatePerKm: 8m,
            driverRatePerKm: 6m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
        await AssertRatesUnchangedAsync(rule.Id);
    }

    [Fact]
    public async Task Handle_When_Rule_Not_Found_Should_Return_DeliveryPricingRuleNotFound_Error()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleRatesCommand(Guid.NewGuid());

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DeliveryPricingRuleNotFound.Code, result.TopError.Code);
    }
}
