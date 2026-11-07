using UberSy.Application.Features.DeliveryPricingRules.Commands.UpdateSchedule;
using UberSy.Test.Common.DeliveryPricingRule;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.DeliveryPricingRules.UpdateSchedule;

public class UpdatePricingRuleScheduleCommandValidatorTests
{
    private readonly UpdatePricingRuleScheduleCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            startHour: new TimeOnly(8, 0),
            endHour: new TimeOnly(20, 0));

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Pass_When_Both_Hours_Are_Null()
    {
        // The factory fills default hours when both are null, so the command is built directly
        var command = new UpdatePricingRuleScheduleCommand(Guid.NewGuid(), null, null);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Have_Error_When_RuleId_Is_Empty()
    {
        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(ruleId: Guid.Empty);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "RuleId");
    }

    [Fact]
    public void Should_Have_Error_When_Only_StartHour_Is_Set()
    {
        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            startHour: new TimeOnly(8, 0),
            endHour: null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("StartHour and EndHour"));
    }

    [Fact]
    public void Should_Have_Error_When_Only_EndHour_Is_Set()
    {
        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            startHour: null,
            endHour: new TimeOnly(20, 0));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("StartHour and EndHour"));
    }

    [Fact]
    public void Should_Have_Error_When_EndHour_Is_Before_StartHour()
    {
        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            startHour: new TimeOnly(20, 0),
            endHour: new TimeOnly(8, 0));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EndHour");
    }

    [Fact]
    public void Should_Have_Error_When_EndHour_Equals_StartHour()
    {
        var command = PricingRuleCommandFactory.CreateUpdatePricingRuleScheduleCommand(
            startHour: new TimeOnly(8, 0),
            endHour: new TimeOnly(8, 0));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EndHour");
    }
}
