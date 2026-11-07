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

namespace UberSy.Application.SubcutaneousTests.Features.DeliveryPricingRules.DeletePricingRule;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class DeletePricingRuleCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public DeletePricingRuleCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private static TbDeliveryPricingRule CreateRule(VehicleType? vehicleType = null)
    {
        var result = DeliveryPricingRuleFactory.CreateDeliveryPricingRule(vehicleType: vehicleType);

        if (result.IsError)
        {
            throw new InvalidOperationException(
                $"DeliveryPricingRuleFactory produced invalid data: {result.TopError.Code} - {result.TopError.Description}");
        }

        return result.Value;
    }

    private async Task<bool> IsDeletedInDbAsync(Guid ruleId)
    {
        var rule = await _context.TbDeliveryPricingRules
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstAsync(r => r.Id == ruleId);

        return rule.IsDeleted;
    }

    [Fact]
    public async Task Handle_When_Admin_Deletes_Rule_Should_Succeed_And_Mark_It_Deleted()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = CreateRule();
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateDeletePricingRuleCommand(rule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");
        Assert.True(await IsDeletedInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_Should_Only_Delete_The_Requested_Rule()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var target = CreateRule(VehicleType.Motorcycle);
        var other = CreateRule(VehicleType.Bicycle);

        _context.TbDeliveryPricingRules.Add(target);
        _context.TbDeliveryPricingRules.Add(other);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateDeletePricingRuleCommand(target.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(await IsDeletedInDbAsync(target.Id));
        Assert.False(await IsDeletedInDbAsync(other.Id));
    }

    [Fact]
    public async Task Handle_When_Rule_Is_Deleted_It_Should_No_Longer_Block_A_New_Overlapping_Rule()
    {
        // Arrange: an active all-day rule that is then deleted
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = CreateRule();
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var deleteResult = await _mediator.Send(
            PricingRuleCommandFactory.CreateDeletePricingRuleCommand(rule.Id));
        Assert.True(deleteResult.IsSuccess);

        // Act: create a rule with the same vehicle/tier and an overlapping schedule
        var createResult = await _mediator.Send(
            PricingRuleCommandFactory.CreateCreatePricingRuleCommand());

        // Assert
        Assert.True(
            createResult.IsSuccess,
            createResult.IsError ? $"{createResult.TopError.Code}: {createResult.TopError.Description}" : "");
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Admin_Should_Return_AccessDenied_And_Not_Delete()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Customer);

        var rule = CreateRule();
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateDeletePricingRuleCommand(rule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
        Assert.False(await IsDeletedInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Authenticated_Should_Return_AccessDenied()
    {
        // Arrange (no user set)
        var rule = CreateRule();
        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateDeletePricingRuleCommand(rule.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
        Assert.False(await IsDeletedInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_When_Rule_Not_Found_Should_Return_DeliveryPricingRuleNotFound_Error()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var command = PricingRuleCommandFactory.CreateDeletePricingRuleCommand(Guid.NewGuid());

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DeliveryPricingRuleNotFound.Code, result.TopError.Code);
    }
}
