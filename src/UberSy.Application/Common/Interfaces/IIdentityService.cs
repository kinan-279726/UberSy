using MediatR;
using UberSy.Application.Features.Identity;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> AuthorizeAsync(Guid userId, string? policyName);

    Task<Result<ApplicationUserDto>> AuthenticateAsync(string email, string password);

    Task<Result<ApplicationUserDto>> GetUserByIdAsync(Guid userId);

    Task<string?> GetUserNameAsync(Guid userId);

    Task<Result<ApplicationUserDto>> RegisterAsync(string email, string password, string nameAr, string nameEn, string phone, string role = Roles.Customer);

    Task<bool> IsInRoleAsync(Guid userId, string role);
}

