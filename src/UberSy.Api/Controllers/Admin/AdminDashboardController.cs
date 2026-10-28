using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Features.Admin.Dtos;
using UberSy.Application.Features.Admin.Queries.AdminDashboard;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers.Admin;

[Route("api/v{version:apiVersion}/admin/dashboard")]
[ApiVersion("1.0")]
[Authorize(Policy = Policies.AdminOnly)]
public sealed class AdminDashboardController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(AdminDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Retrieves the admin dashboard.")]
    [EndpointDescription("Order counts, sales, platform revenue and live operational numbers. Defaults to today (Damascus time).")]
    [EndpointName("GetAdminDashboard")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAdminDashboardQuery(from, to), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }
}