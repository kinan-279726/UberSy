using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Constants;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Infrastructure.Services;

public sealed class OwnershipValidator(IUser user, IApplicationDbContext context, IHttpContextAccessor accessor) : IOwnershipValidator
{
    private readonly IUser _user = user;
    private readonly IApplicationDbContext _context = context;
    private readonly IHttpContextAccessor _accessor = accessor;

    public bool IsInRole(string role)
    {
        if (IsAnonymous()) return false;

        var result = _accessor.HttpContext?.User.IsInRole(role);
        if (result is null) return false;

        return result.Value;
    }

    public bool IsAnonymous() => string.IsNullOrWhiteSpace(_user.Id);

    public async Task<Result<Guid>> IsUser()
    {
        if (IsAnonymous())
        {
            return ApplicationErrors.Unauthorized;

        }

        return Guid.Parse(_user.Id!);
    }

    public async Task<Result<Guid>> IsDriver()
    {
        if (IsAnonymous())
        {
            return ApplicationErrors.Unauthorized;

        }

        var driverId = await _context.TbDrivers.AsNoTracking()
                                               .FirstOrDefaultAsync(d => d.UserId == Guid.Parse(_user.Id!));

        if (driverId is null) return ApplicationErrors.DriverNotFound;

        return driverId.Id;
    }

    public Result<bool> EnsureIsAManager(CancellationToken cancellationToken = default)
    {
        if (IsAnonymous() || !_user.IsInRole(Roles.Admin))
            return ApplicationErrors.AccessDenied;

        return true;
    }

    public async Task<Result<bool>> EnsureIsOwnerByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        if (IsAnonymous())
            return ApplicationErrors.Unauthorized;

        var userId = Guid.Parse(_user.Id!);

        var restaurant = await _context.TbRestaurants.AsNoTracking()
                                                     .FirstOrDefaultAsync(r => r.OwnerId == userId && r.Id == restaurantId, cancellationToken);

        if (restaurant is null) return ApplicationErrors.RestaurantNotFound;

        return true;
    }

    public async Task<Result<bool>> EnsureIsOwnerByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        if (IsAnonymous())
            return ApplicationErrors.Unauthorized;

        var isManager = EnsureIsAManager(cancellationToken);
        if (isManager.IsSuccess)
            return true;

        if (ownerId == Guid.Parse(_user.Id!)) return true;

        return ApplicationErrors.AccessDenied;
    }

    public async Task<Result<Guid>> EnsureCanEditAddress(Guid addressId, CancellationToken cancellationToken = default)
    {
        if (IsAnonymous()) return ApplicationErrors.Unauthorized;

        var address = await _context.TbAddresses.AsNoTracking().FirstOrDefaultAsync(a => a.Id == addressId);

        if (address is null) return ApplicationErrors.AddressNotFound;

        if (Guid.Parse(_user.Id!) != address.UserId) return ApplicationErrors.AccessDenied;

        return Guid.Parse(_user.Id!);
    }
}