using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Admin.Dtos;
using UberSy.Application.Features.Admin.Queries.GetDrivers;
using UberSy.Application.Features.Drivers.Commands.ApproveDriver;
using UberSy.Contract.Requests.Drivers;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers.Enums;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers;

[Route("api/v{version:apiVersion}/admin/drivers")]
[ApiVersion("1.0")]
[Authorize(Policy = Policies.AdminOnly)]
public sealed class AdminDriversController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<AdminDriverListItemDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves drivers (paged).")]
    [EndpointDescription("Filter by approval, status, and national id or plate number. Use isApproved=false for pending drivers.")]
    [EndpointName("GetDrivers")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAll(
    [FromQuery] bool? isApproved,
    [FromQuery] DriverStatus? status,
    [FromQuery] string? search,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetDriversQuery(isApproved, status, search, pageNumber, pageSize), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("pending")]
    [ProducesResponseType(typeof(PagedList<AdminDriverListItemDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves drivers awaiting approval (paged).")]
    [EndpointDescription("Shortcut for isApproved=false, oldest first.")]
    [EndpointName("GetPendingDrivers")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetPending(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetDriversQuery(false, null, null, pageNumber, pageSize), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("{driverId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Approves a driver.")]
    [EndpointDescription("The admin decides the service tier (Standard or Premium). Grants the Driver role.")]
    [EndpointName("ApproveDriver")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Approve(
    Guid driverId,
    [FromBody] ApproveDriverRequest request,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ApproveDriverCommand(driverId, (ServiceTier)request.ServiceTier), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}