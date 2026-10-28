using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UberSy.Application.Features.Identity;
using UberSy.Application.Features.Identity.Commands.ConfirmEmail;
using UberSy.Application.Features.Identity.Commands.ConfirmPhone;
using UberSy.Application.Features.Identity.Commands.Register;
using UberSy.Application.Features.Identity.Commands.ResendEmailCode;
using UberSy.Application.Features.Identity.Commands.SendPhoneCode;
using UberSy.Application.Features.Identity.Queries.GenerateTokens;
using UberSy.Application.Features.Identity.Queries.GetUserInfo;
using UberSy.Application.Features.Identity.Queries.RefreshTokens;
using UberSy.Contract.Requests.Identity;

namespace UberSy.Api.Controllers;

[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
public sealed class AuthController(ISender sender) : ApiController
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApplicationUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Registers a new customer account.")]
    [EndpointDescription("Creates a Customer account. A verification code is required before the first login.")]
    [EndpointName("Register")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RegisterCommand(request.Email, request.Password, request.NameAr, request.NameEn, request.Phone),
            cancellationToken);

        return result.Match(response => StatusCode(StatusCodes.Status201Created, response), Problem);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Authenticates a user.")]
    [EndpointDescription("Returns an access token and a refresh token. The email must be confirmed.")]
    [EndpointName("Login")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GenerateTokenQuery(request.Email, request.Password), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Refreshes an access token.")]
    [EndpointDescription("Exchanges a valid refresh token and an expired access token for a new pair.")]
    [EndpointName("RefreshToken")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RefreshTokenQuery(request.RefreshToken, request.ExpiredAccessToken), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApplicationUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Retrieves the current user.")]
    [EndpointDescription("Returns the authenticated user's id, email, roles and claims.")]
    [EndpointName("GetCurrentUser")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var result = await sender.Send(new GetUserByIdQuery(userId), cancellationToken);

        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [EnableRateLimiting("otp")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [EndpointSummary("Confirms an email address.")]
    [EndpointDescription("Confirms the email with the 6-digit code sent to it.")]
    [EndpointName("ConfirmEmail")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> ConfirmEmail(
        [FromBody] ConfirmEmailRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ConfirmEmailCommand(request.Email, request.Code), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("resend-email-code")]
    [AllowAnonymous]
    [EnableRateLimiting("otp")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [EndpointSummary("Resends the email verification code.")]
    [EndpointDescription("Always returns 204 to avoid revealing whether the email exists.")]
    [EndpointName("ResendEmailCode")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> ResendEmailCode(
        [FromBody] ResendEmailCodeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ResendEmailCodeCommand(request.Email), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("phone/send-code")]
    [Authorize]
    [EnableRateLimiting("otp")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [EndpointSummary("Sends an SMS verification code.")]
    [EndpointDescription("Sends a 6-digit code to the current user's phone number.")]
    [EndpointName("SendPhoneCode")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> SendPhoneCode(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SendPhoneCodeCommand(), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("phone/confirm")]
    [Authorize]
    [EnableRateLimiting("otp")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [EndpointSummary("Confirms the phone number.")]
    [EndpointDescription("Confirms the current user's phone with the 6-digit SMS code.")]
    [EndpointName("ConfirmPhone")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> ConfirmPhone(
        [FromBody] ConfirmPhoneRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ConfirmPhoneCommand(request.Code), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}