using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.DeliveryPricingRules.Commands.UpdateSchedule;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.DeliveryRule;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Identity;
using UberSy.Test.Common.DeliveryPricingRule;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.DeliveryPricingRules.UpdateSchedule;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdatePricingRuleScheduleCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public UpdatePricingRuleScheduleCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<TbDeliveryPricingRule> AddRuleAsync(
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

        return rule;
    }

    private Task<TbDeliveryPricingRule> GetRuleFromDbAsync(Guid ruleId)
        => _context.TbDeliveryPricingRules.AsNoTracking().FirstAsync(r => r.Id == ruleId);

    [Fact]
    public async Task Handle_When_Admin_Updates_Schedule_Should_Succeed_And_Persist()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync(startHour: new TimeOnly(8, 0), endHour: new TimeOnly(12, 0));

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            rule.Id,
            startHour: new TimeOnly(9, 0),
            endHour: new TimeOnly(18, 0));

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var updated = await GetRuleFromDbAsync(rule.Id);
        Assert.Equal(new TimeOnly(9, 0), updated.StartHour);
        Assert.Equal(new TimeOnly(18, 0), updated.EndHour);
    }

    [Fact]
    public async Task Handle_When_Both_Hours_Are_Null_Should_Clear_The_Schedule()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync(startHour: new TimeOnly(8, 0), endHour: new TimeOnly(12, 0));

        // Built directly because the factory fills default hours when both are null
        var command = new UpdatePricingRuleScheduleCommand(rule.Id, null, null);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var updated = await GetRuleFromDbAsync(rule.Id);
        Assert.Null(updated.StartHour);
        Assert.Null(updated.EndHour);
    }

    [Fact]
    public async Task Handle_When_New_Schedule_Overlaps_Another_Active_Rule_Should_Return_Overlap_Error()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        await AddRuleAsync(startHour: new TimeOnly(8, 0), endHour: new TimeOnly(12, 0));
        var target = await AddRuleAsync(startHour: new TimeOnly(14, 0), endHour: new TimeOnly(18, 0));

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            target.Id,
            startHour: new TimeOnly(10, 0),
            endHour: new TimeOnly(16, 0));

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.PricingRuleScheduleOverlap.Code, result.TopError.Code);

        var unchanged = await GetRuleFromDbAsync(target.Id);
        Assert.Equal(new TimeOnly(14, 0), unchanged.StartHour);
        Assert.Equal(new TimeOnly(18, 0), unchanged.EndHour);
    }

    [Fact]
    public async Task Handle_When_Clearing_Schedule_Of_Rule_With_Active_Sibling_Should_Return_Overlap_Error()
    {
        // Arrange: no hours means all day, which overlaps any other active rule of the same kind
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        await AddRuleAsync(startHour: new TimeOnly(8, 0), endHour: new TimeOnly(12, 0));
        var target = await AddRuleAsync(startHour: new TimeOnly(14, 0), endHour: new TimeOnly(18, 0));

        var command = new UpdatePricingRuleScheduleCommand(target.Id, null, null);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.PricingRuleScheduleOverlap.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_When_New_Schedule_Overlaps_Only_The_Rule_Itself_Should_Succeed()
    {
        // Arrange: the rule is excluded from its own overlap check
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync(startHour: new TimeOnly(14, 0), endHour: new TimeOnly(18, 0));

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            rule.Id,
            startHour: new TimeOnly(15, 0),
            endHour: new TimeOnly(19, 0));

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_When_Schedules_Only_Touch_At_The_Boundary_Should_Succeed()
    {
        // Arrange: existing 08:00-12:00, the target moves to start exactly at 12:00
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        await AddRuleAsync(startHour: new TimeOnly(8, 0), endHour: new TimeOnly(12, 0));
        var target = await AddRuleAsync(startHour: new TimeOnly(14, 0), endHour: new TimeOnly(18, 0));

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            target.Id,
            startHour: new TimeOnly(12, 0),
            endHour: new TimeOnly(18, 0));

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_When_Overlapping_Rule_Has_Different_VehicleType_Should_Succeed()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        await AddRuleAsync(vehicleType: VehicleType.Motorcycle);
        var target = await AddRuleAsync(
            vehicleType: VehicleType.Bicycle,
            startHour: new TimeOnly(14, 0),
            endHour: new TimeOnly(18, 0));

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            target.Id,
            startHour: new TimeOnly(10, 0),
            endHour: new TimeOnly(12, 0));

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_When_Rule_Is_Inactive_Should_Skip_The_Overlap_Check()
    {
        // Arrange: the handler only checks overlaps for active rules
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        await AddRuleAsync(startHour: new TimeOnly(8, 0), endHour: new TimeOnly(12, 0));
        var target = await AddRuleAsync(
            isActive: false,
            startHour: new TimeOnly(14, 0),
            endHour: new TimeOnly(18, 0));

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            target.Id,
            startHour: new TimeOnly(10, 0),
            endHour: new TimeOnly(16, 0));

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var updated = await GetRuleFromDbAsync(target.Id);
        Assert.Equal(new TimeOnly(10, 0), updated.StartHour);
    }

    [Fact]
    public async Task Handle_With_EndHour_Before_StartHour_Should_Return_Validation_Error_And_Not_Update()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var rule = await AddRuleAsync(startHour: new TimeOnly(8, 0), endHour: new TimeOnly(12, 0));

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            rule.Id,
            startHour: new TimeOnly(20, 0),
            endHour: new TimeOnly(8, 0));

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "EndHour");

        var unchanged = await GetRuleFromDbAsync(rule.Id);
        Assert.Equal(new TimeOnly(8, 0), unchanged.StartHour);
        Assert.Equal(new TimeOnly(12, 0), unchanged.EndHour);
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Admin_Should_Return_AccessDenied_And_Not_Update()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Customer);

        var rule = await AddRuleAsync(startHour: new TimeOnly(8, 0), endHour: new TimeOnly(12, 0));

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            rule.Id,
            startHour: new TimeOnly(9, 0),
            endHour: new TimeOnly(18, 0));

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);

        var unchanged = await GetRuleFromDbAsync(rule.Id);
        Assert.Equal(new TimeOnly(8, 0), unchanged.StartHour);
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Authenticated_Should_Return_AccessDenied()
    {
        // Arrange (no user set)
        var rule = await AddRuleAsync(startHour: new TimeOnly(8, 0), endHour: new TimeOnly(12, 0));

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            rule.Id,
            startHour: new TimeOnly(9, 0),
            endHour: new TimeOnly(18, 0));

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

        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(Guid.NewGuid());

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DeliveryPricingRuleNotFound.Code, result.TopError.Code);
    }
}
