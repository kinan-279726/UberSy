using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.DeliveryRule;
using UberSy.Domain.Identity;
using UberSy.Test.Common.DeliveryPricingRule;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.DeliveryPricingRules.UpdatePricingRuleMaxDistance;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdatePricingRuleMaxDistanceCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public UpdatePricingRuleMaxDistanceCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<TbDeliveryPricingRule> AddRuleAsync(decimal? maxDistanceKm = null)
    {
        var result = DeliveryPricingRuleFactory.CreateDeliveryPricingRule(maxDistanceKm: maxDistanceKm);

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

    private async Task<decimal?> GetMaxDistanceInDbAsync(Guid ruleId)
    {
        var rule = await _context.TbDeliveryPricingRules.AsNoTracking().FirstAsync(r => r.Id == ruleId);
        return rule.MaxDistanceKm;
    }

    [Fact]
    public async Task Handle_When_Admin_Sets_MaxDistance_Should_Succeed_And_Persist()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync(maxDistanceKm: 10m);

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleMaxDistanceCommand(rule.Id, 25m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");
        Assert.Equal(25m, await GetMaxDistanceInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_When_Admin_Sets_MaxDistance_To_Null_Should_Remove_The_Limit()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync(maxDistanceKm: 10m);

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleMaxDistanceCommand(rule.Id, null);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(await GetMaxDistanceInDbAsync(rule.Id));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_When_MaxDistance_Is_Not_Positive_Should_Return_Domain_Error_And_Not_Update(double value)
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync(maxDistanceKm: 10m);

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleMaxDistanceCommand(rule.Id, (decimal)value);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DeliveryRuleErrors.MaxDistanceKmInvalid.Code, result.TopError.Code);
        Assert.Equal(10m, await GetMaxDistanceInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_Should_Only_Update_The_Requested_Rule()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var target = await AddRuleAsync(maxDistanceKm: 10m);

        var otherResult = DeliveryPricingRuleFactory.CreateDeliveryPricingRule(
            vehicleType: UberSy.Domain.DeliveryRule.Enums.VehicleType.Bicycle,
            maxDistanceKm: 7m);
        var other = otherResult.Value;
        _context.TbDeliveryPricingRules.Add(other);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleMaxDistanceCommand(target.Id, 30m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(30m, await GetMaxDistanceInDbAsync(target.Id));
        Assert.Equal(7m, await GetMaxDistanceInDbAsync(other.Id));
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Admin_Should_Return_AccessDenied_And_Not_Update()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Customer);

        var rule = await AddRuleAsync(maxDistanceKm: 10m);

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleMaxDistanceCommand(rule.Id, 25m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
        Assert.Equal(10m, await GetMaxDistanceInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Authenticated_Should_Return_AccessDenied()
    {
        // Arrange (no user set)
        var rule = await AddRuleAsync(maxDistanceKm: 10m);

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleMaxDistanceCommand(rule.Id, 25m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
        Assert.Equal(10m, await GetMaxDistanceInDbAsync(rule.Id));
    }

    [Fact]
    public async Task Handle_When_Rule_Not_Found_Should_Return_DeliveryPricingRuleNotFound_Error()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleMaxDistanceCommand(Guid.NewGuid(), 25m);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DeliveryPricingRuleNotFound.Code, result.TopError.Code);
    }
}
