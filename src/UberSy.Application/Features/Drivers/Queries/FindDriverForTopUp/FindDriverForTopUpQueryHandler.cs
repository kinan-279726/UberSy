using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Application.Features.Identity.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Queries.FindDriverForTopUp;

public sealed class FindDriverForTopUpQueryHandler(
    IApplicationDbContext context,
    IUserManagementService users,
    IUser user)
    : IRequestHandler<FindDriverForTopUpQuery, Result<AgentDriverLookupDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUserManagementService _users = users;
    private readonly IUser _user = user;

    public async Task<Result<AgentDriverLookupDto>> Handle(
        FindDriverForTopUpQuery request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Domain.Identity.Roles.Agent)) return ApplicationErrors.AccessDenied;

        var search = request.Search.Trim();

        var driver = await _context.TbDrivers
            .AsNoTracking()
            .Include(d => d.DriverWallet)
            .FirstOrDefaultAsync(d => d.NationalId == search, cancellationToken);

        UserContactDto? contact = null;

        if (driver is null)
        {
            contact = await _users.FindContactByPhoneAsync(search, cancellationToken);

            if (contact is not null)
            {
                driver = await _context.TbDrivers
                    .AsNoTracking()
                    .Include(d => d.DriverWallet)
                    .FirstOrDefaultAsync(d => d.UserId == contact.UserId, cancellationToken);
            }
        }

        if (driver is null) return ApplicationErrors.DriverNotFound;

        contact ??= await _users.GetContactAsync(driver.UserId, cancellationToken);
        if (contact is null) return ApplicationErrors.UserNotFound;

        var nationalId = driver.NationalId;
        var masked = nationalId.Length <= 4
            ? nationalId
            : new string('*', nationalId.Length - 4) + nationalId[^4..];

        return new AgentDriverLookupDto
        {
            DriverId = driver.Id,
            NameAr = contact.NameAr,
            NameEn = contact.NameEn,
            Phone = contact.Phone,
            NationalIdMasked = masked,
            PhotoUrl = driver.PhotoUrl,
            VehicleType = driver.VehicleType,
            PlateNumber = driver.PlateNumber,
            IsApproved = driver.IsApproved,
            CurrentBalance = driver.DriverWallet?.Balance ?? 0m,
            CanTopUp = driver.IsApproved && driver.DriverWallet is not null,
        };
    }
}