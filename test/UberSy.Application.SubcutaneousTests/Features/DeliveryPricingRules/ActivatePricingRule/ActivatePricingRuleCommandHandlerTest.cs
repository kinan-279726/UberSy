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

namespace UberSy.Application.SubcutaneousTests.Features.DeliveryPricingRules.ActivatePricingRule;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class ActivatePricingRuleCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public ActivatePricingRuleCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private static TbDeliveryPricingRule CreateRule(
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

        return rule;
    }

    private async Task<bool> IsActiveInDbAsync(Guid ruleId)
    {
        var rule = await _context.TbDeliveryPricingRules.AsNoTracking().FirstAsync(r => r.Id == ruleId);
        return rule.IsActive;
    }

    [Fact]
    public async Task Handle_When_Admin_Activates_Inactive_Rule_Should_Succeed()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = CreateRule(isActive: false);
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateActivatePricingRuleCommand(rule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");
        Assert.True(await IsActiveInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_When_Rule_Is_Already_Active_Should_Return_Success_And_Stay_Active()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = CreateRule(isActive: true);
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateActivatePricingRuleCommand(rule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(await IsActiveInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Admin_Should_Return_AccessDenied_And_Not_Activate()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Customer);

        var rule = CreateRule(isActive: false);
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateActivatePricingRuleCommand(rule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
        Assert.False(await IsActiveInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Authenticated_Should_Return_AccessDenied()
    {
        // Arrange
        var rule = CreateRule(isActive: false);
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateActivatePricingRuleCommand(rule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_When_Rule_Not_Found_Should_Return_DeliveryPricingRuleNotFound_Error()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var command = PricingRuleCommandFactory.CreateActivatePricingRuleCommand(Guid.NewGuid());

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DeliveryPricingRuleNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_When_Schedule_Overlaps_Active_Rule_Should_Return_Overlap_Error_And_Not_Activate()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var activeRule = CreateRule(isActive: true, startHour: new TimeOnly(8, 0), endHour: new TimeOnly(16, 0));
        var inactiveRule = CreateRule(isActive: false, startHour: new TimeOnly(12, 0), endHour: new TimeOnly(20, 0));

        _context.TbDeliveryPricingRules.Add(activeRule);
        _context.TbDeliveryPricingRules.Add(inactiveRule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateActivatePricingRuleCommand(inactiveRule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.PricingRuleScheduleOverlap.Code, result.TopError.Code);
        Assert.False(await IsActiveInDbAsync(inactiveRule.Id));
    }

    [Fact]
    public async Task Handle_When_Schedules_Only_Touch_At_The_Boundary_Should_Activate()
    {
        // Arrange: 08:00-12:00 
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var activeRule = CreateRule(isActive: true, startHour: new TimeOnly(8, 0), endHour: new TimeOnly(12, 0));
        var inactiveRule = CreateRule(isActive: false, startHour: new TimeOnly(12, 0), endHour: new TimeOnly(20, 0));

        _context.TbDeliveryPricingRules.Add(activeRule);
        _context.TbDeliveryPricingRules.Add(inactiveRule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateActivatePricingRuleCommand(inactiveRule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(await IsActiveInDbAsync(inactiveRule.Id));
    }

    [Fact]
    public async Task Handle_When_Overlapping_Rule_Has_Different_VehicleType_Should_Activate()
    {
        // Arrange: 
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var activeMotorcycle = CreateRule(isActive: true, vehicleType: VehicleType.Motorcycle);
        var inactiveBicycle = CreateRule(isActive: false, vehicleType: VehicleType.Bicycle);

        _context.TbDeliveryPricingRules.Add(activeMotorcycle);
        _context.TbDeliveryPricingRules.Add(inactiveBicycle);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateActivatePricingRuleCommand(inactiveBicycle.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(await IsActiveInDbAsync(inactiveBicycle.Id));
    }
}
