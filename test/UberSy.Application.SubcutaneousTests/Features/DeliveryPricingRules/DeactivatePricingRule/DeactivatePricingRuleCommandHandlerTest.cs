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

namespace UberSy.Application.SubcutaneousTests.Features.DeliveryPricingRules.DeactivatePricingRule;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class DeactivatePricingRuleCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public DeactivatePricingRuleCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private static TbDeliveryPricingRule CreateRule(bool isActive = true, VehicleType? vehicleType = null)
    {
        var result = DeliveryPricingRuleFactory.CreateDeliveryPricingRule(vehicleType: vehicleType);

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
    public async Task Handle_When_Admin_Deactivates_Active_Rule_Should_Succeed()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = CreateRule(isActive: true);
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateDeactivatePricingRuleCommand(rule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");
        Assert.False(await IsActiveInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_When_Rule_Is_Already_Inactive_Should_Return_Success_And_Stay_Inactive()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = CreateRule(isActive: false);
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateDeactivatePricingRuleCommand(rule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(await IsActiveInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_Should_Only_Deactivate_The_Requested_Rule()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var target = CreateRule(isActive: true, vehicleType: VehicleType.Motorcycle);
        var other = CreateRule(isActive: true, vehicleType: VehicleType.Bicycle);

        _context.TbDeliveryPricingRules.Add(target);
        _context.TbDeliveryPricingRules.Add(other);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateDeactivatePricingRuleCommand(target.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(await IsActiveInDbAsync(target.Id));
        Assert.True(await IsActiveInDbAsync(other.Id));
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Admin_Should_Return_AccessDenied_And_Not_Deactivate()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Customer);

        var rule = CreateRule(isActive: true);
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateDeactivatePricingRuleCommand(rule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
        Assert.True(await IsActiveInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Authenticated_Should_Return_AccessDenied()
    {
        // Arrange (no user set)
        var rule = CreateRule(isActive: true);
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateDeactivatePricingRuleCommand(rule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
        Assert.True(await IsActiveInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_When_Rule_Not_Found_Should_Return_DeliveryPricingRuleNotFound_Error()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var command = PricingRuleCommandFactory.CreateDeactivatePricingRuleCommand(Guid.NewGuid());

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DeliveryPricingRuleNotFound.Code, result.TopError.Code);
    }
}
