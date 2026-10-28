using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Net;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Infrastructure.Identity;

public sealed class VerificationService(
UserManager<ApplicationUser> userManager,
IEmailSender emailSender,
ISmsSender smsSender,
ILogger<VerificationService> logger) : IVerificationService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ISmsSender _smsSender = smsSender;
    private readonly ILogger<VerificationService> _logger = logger;
    private const string EmailPurpose = "ConfirmEmail";

    public async Task<Result<Updated>> ConfirmEmailAsync(string email, string code)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim());

        if (user is null) return ApplicationErrors.InvalidVerificationCode;
        if (user.EmailConfirmed) return Result.Updated;
        if (await _userManager.IsLockedOutAsync(user)) return ApplicationErrors.TooManyAttempts;

        var valid = await _userManager.VerifyUserTokenAsync(
            user, TokenOptions.DefaultEmailProvider, EmailPurpose, code.Trim());

        if (!valid)
        {
            await _userManager.AccessFailedAsync(user);
            return ApplicationErrors.InvalidVerificationCode;
        }

        user.EmailConfirmed = true;
        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded) return ApplicationErrors.VerificationFailed;

        await _userManager.ResetAccessFailedCountAsync(user);

        return Result.Updated;
    }

    public async Task<Result<Updated>> ConfirmPhoneAsync(Guid userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return ApplicationErrors.UserNotFound;
        if (string.IsNullOrWhiteSpace(user.PhoneNumber)) return ApplicationErrors.PhoneRequired;
        if (user.PhoneNumberConfirmed) return Result.Updated;
        if (await _userManager.IsLockedOutAsync(user)) return ApplicationErrors.TooManyAttempts;

        var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, code.Trim());

        if (!result.Succeeded)
        {
            await _userManager.AccessFailedAsync(user);
            return ApplicationErrors.InvalidVerificationCode;
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        return Result.Updated;
    }

    public async Task<Result<Updated>> SendEmailCodeAsync(string email, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim());

        if (user is null) return ApplicationErrors.UserNotFound;

        if(user.EmailConfirmed) return Result.Updated;

        var code = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultEmailProvider, EmailPurpose);

        var send = await _emailSender.SendAsync(
            user.Email!, "رمز تأكيد البريد | Email verification code", BuildEmailBody(user.NameAr, code), ct);

        if (send.IsError) return send.Errors;

        return Result.Updated;

    }

    public async Task<Result<Updated>> SendPhoneCodeAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null) return ApplicationErrors.UserNotFound;
        if (string.IsNullOrWhiteSpace(user.PhoneNumber)) return ApplicationErrors.PhoneRequired;
        if (user.PhoneNumberConfirmed) return ApplicationErrors.PhoneAlreadyConfirmed;

        var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

        var send = await _smsSender.SendAsync(user.PhoneNumber, $"UberSy code: {code}", ct);

        if (send.IsError) return send.Errors;

        return Result.Updated;

    }

    private static string BuildEmailBody(string name, string code) => $"""
        <div dir="rtl" style="font-family:Tahoma,Arial,sans-serif">
          <p>مرحباً {WebUtility.HtmlEncode(name)}،</p>
          <p>رمز تأكيد بريدك الإلكتروني في UberSy:</p>
          <h2 style="letter-spacing:6px">{code}</h2>
          <p>الرمز صالح لدقائق قليلة. إذا لم تطلبه، تجاهل هذه الرسالة.</p>
          <hr/>
          <p dir="ltr">Your UberSy email verification code is shown above. It expires in a few minutes.</p>
        </div>
        """;
}
