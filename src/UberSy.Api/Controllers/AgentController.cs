using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UberSy.Application.Features.Drivers.Commands.TopUpWallet;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Application.Features.Drivers.Queries.FindDriverForTopUp;
using UberSy.Contract.Requests.Drivers;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers;

[Route("api/v{version:apiVersion}/agent/drivers")]
[ApiVersion("1.0")]
[Authorize(Policy = Policies.AgentOnly)]
public sealed class AgentController(ISender sender) : ApiController
{
    [HttpGet("lookup")]
    [EnableRateLimiting("agent-lookup")]
    [ProducesResponseType(typeof(AgentDriverLookupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Finds a driver by national id or phone number.")]
    [EndpointDescription("Exact match only. Returns the driver's identity details and current wallet balance so the agent can verify before topping up.")]
    [EndpointName("FindDriverForTopUp")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Lookup([FromQuery] string search, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new FindDriverForTopUpQuery(search), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("{driverId:guid}/wallet/top-up")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Tops up a driver's wallet.")]
    [EndpointDescription("Records the agent as the processor of the transaction.")]
    [EndpointName("TopUpDriverWallet")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> TopUp(
        Guid driverId,
        [FromBody] TopUpWalletRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new TopUpWalletCommand(request.Amount, driverId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}