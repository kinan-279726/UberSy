using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Admin.Commands.BanUser;
using UberSy.Application.Features.Admin.Commands.UnbanUser;
using UberSy.Application.Features.Admin.Queries.Users;
using UberSy.Application.Features.Identity;
using UberSy.Application.Features.Identity.Dtos;
using UberSy.Application.Features.Identity.Roles.Commands.AssignRoleToUser;
using UberSy.Application.Features.Identity.Roles.Commands.RemoveRoleFromUser;
using UberSy.Application.Features.Identity.Roles.Queries.GetAllRoles;
using UberSy.Application.Features.Identity.Roles.Queries.GetUserRoles;
using UberSy.Application.Features.Identity.Roles.Queries.GetUsersByRole;
using UberSy.Contract.Requests.Admin;
using UberSy.Contract.Requests.Identity;
using UberSy.Infrastructure.Identity;

namespace UberSy.Api.Controllers.Admin;

[Route("api/v{version:apiVersion}/admin")]
[ApiVersion("1.0")]
[Authorize(Policy = Policies.AdminOnly)]
public sealed class AdminUsersController(ISender sender) : ApiController
{
    [HttpGet("roles")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves all roles.")]
    [EndpointDescription("Returns every role defined in the system.")]
    [EndpointName("GetAllRoles")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllRolesQuery(), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("roles/{role}/users")]
    [ProducesResponseType(typeof(List<ApplicationUserGuidDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves users in a role.")]
    [EndpointDescription("Paged by page and pageSize.")]
    [EndpointName("GetUsersByRole")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetUsersByRole(
        string role,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetUsersByRoleQuery(role, page, pageSize), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("{userId:guid}/roles")]
    [ProducesResponseType(typeof(ApplicationUserGuidDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves a user's roles.")]
    [EndpointDescription("Returns the user with all assigned roles.")]
    [EndpointName("GetUserRoles")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetUserRoles(Guid userId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUserRolesQuery(userId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("{userId:guid}/roles")]
    [ProducesResponseType(typeof(ApplicationUserGuidDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Assigns a role to a user.")]
    [EndpointDescription("Returns the user with the updated roles.")]
    [EndpointName("AssignRoleToUser")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> AssignRole(
    Guid userId,
    [FromBody] AssignRoleRequest request,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AssignRoleToUserCommand(userId, request.Role), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpDelete("{userId:guid}/roles/{role}")]
    [ProducesResponseType(typeof(ApplicationUserGuidDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Removes a role from a user.")]
    [EndpointDescription("Returns the user with the updated roles.")]
    [EndpointName("RemoveRoleFromUser")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> RemoveRole(Guid userId, string role, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveRoleFromUserCommand(userId, role), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("users")]
    [ProducesResponseType(typeof(PagedList<AdminUserListItemDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieves users (paged).")]
    [EndpointDescription("Search by email, name or phone. Filter by role and ban state.")]
    [EndpointName("GetUsers")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetUsers(
    [FromQuery] string? search,
    [FromQuery] string? role,
    [FromQuery] bool? isBanned,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetUsersQuery(search, role, isBanned, pageNumber, pageSize), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("users/{userId:guid}/ban")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Bans a user.")]
    [EndpointDescription("Until empty means an open-ended ban. Revokes refresh tokens. Admins cannot be banned.")]
    [EndpointName("BanUser")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Ban(
        Guid userId,
        [FromBody] BanUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new BanUserCommand(userId, request.Until), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("users/{userId:guid}/unban")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("Lifts a user's ban.")]
    [EndpointDescription("The user can log in again.")]
    [EndpointName("UnbanUser")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Unban(Guid userId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UnbanUserCommand(userId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}