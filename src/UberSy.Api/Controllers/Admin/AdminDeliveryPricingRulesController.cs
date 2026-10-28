using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Features.DeliveryPricingRules.Commands.ActivatePricingRule;
using UberSy.Application.Features.DeliveryPricingRules.Commands.CreatePricingRule;
using UberSy.Application.Features.DeliveryPricingRules.Commands.DeactivatePricingRule;
using UberSy.Application.Features.DeliveryPricingRules.Commands.DeletePricingRule;
using UberSy.Application.Features.DeliveryPricingRules.Commands.UpdatePricingRuleMaxDistance;
using UberSy.Application.Features.DeliveryPricingRules.Commands.UpdateRates;
using UberSy.Application.Features.DeliveryPricingRules.Commands.UpdateSchedule;
using UberSy.Application.Features.DeliveryPricingRules.Dtos;
using UberSy.Application.Features.DeliveryPricingRules.Queries.GetPricingRuleById;
using UberSy.Application.Features.DeliveryPricingRules.Queries.GetPricingRules;
using UberSy.Contract.Requests.DeliveryPricingRules;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers.Admin;

[Route("api/v{version:apiVersion}/admin/pricing-rules")]
[ApiVersion("1.0")]
[Authorize(Policy = Policies.AdminOnly)]
public sealed class AdminDeliveryPricingRulesController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<DeliveryPricingRuleDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves delivery pricing rules.")]
    [EndpointDescription("Optionally filter by vehicle type and active state.")]
    [EndpointName("GetPricingRules")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAll(
        [FromQuery] VehicleType? vehicleType,
        [FromQuery] bool? isActive,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPricingRulesQuery(vehicleType, isActive), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("{ruleId:guid}", Name = "GetPricingRuleById")]
    [ProducesResponseType(typeof(DeliveryPricingRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves a pricing rule by id.")]
    [EndpointDescription("Returns a single delivery pricing rule.")]
    [EndpointName("GetPricingRuleById")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetById(Guid ruleId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPricingRuleByIdQuery(ruleId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost]
    [ProducesResponseType(typeof(DeliveryPricingRuleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Creates a pricing rule.")]
    [EndpointDescription("Fails with 409 if the hours overlap an active rule of the same vehicle configuration.")]
    [EndpointName("CreatePricingRule")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create(
        [FromBody] CreatePricingRuleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePricingRuleCommand(
           (VehicleType)request.VehicleType, (ServiceTier)request.ServiceTier, request.CarType is null ? null : (CarType)request.CarType,
            request.CustomerRatePerKm, request.DriverRatePerKm, request.DriverRatePerOrder,
            request.MaxDistanceKm, request.StartHour, request.EndHour);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            response => CreatedAtRoute("GetPricingRuleById", new { version = "1.0", ruleId = response.Id }, response),
            Problem);
    }

    [HttpPut("{ruleId:guid}/rates")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates pricing rates.")]
    [EndpointDescription("Updates customer/driver rate per km and the optional driver rate per order.")]
    [EndpointName("UpdatePricingRuleRates")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateRates(
        Guid ruleId,
        [FromBody] UpdatePricingRuleRatesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdatePricingRuleRatesCommand(ruleId, request.CustomerRatePerKm,
                request.DriverRatePerKm, request.DriverRatePerOrder),
            cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("{ruleId:guid}/schedule")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Updates the rule's active hours.")]
    [EndpointDescription("Both hours empty means all day. Fails with 409 on overlap.")]
    [EndpointName("UpdatePricingRuleSchedule")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateSchedule(
        Guid ruleId,
        [FromBody] UpdatePricingRuleScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdatePricingRuleScheduleCommand(ruleId, request.StartHour, request.EndHour),
            cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("{ruleId:guid}/max-distance")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates the maximum delivery distance.")]
    [EndpointDescription("Empty means unlimited.")]
    [EndpointName("UpdatePricingRuleMaxDistance")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateMaxDistance(
        Guid ruleId,
        [FromBody] UpdatePricingRuleMaxDistanceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdatePricingRuleMaxDistanceCommand(ruleId, request.MaxDistanceKm), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{ruleId:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Activates a pricing rule.")]
    [EndpointDescription("Fails with 409 if its hours overlap another active rule.")]
    [EndpointName("ActivatePricingRule")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Activate(Guid ruleId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivatePricingRuleCommand(ruleId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{ruleId:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Deactivates a pricing rule.")]
    [EndpointDescription("The rule is ignored when calculating delivery prices.")]
    [EndpointName("DeactivatePricingRule")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Deactivate(Guid ruleId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivatePricingRuleCommand(ruleId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpDelete("{ruleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Deletes a pricing rule.")]
    [EndpointDescription("Soft-deletes the rule. Frozen fees of past orders are not affected.")]
    [EndpointName("DeletePricingRule")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(Guid ruleId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeletePricingRuleCommand(ruleId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}