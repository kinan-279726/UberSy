using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using System.Net.Http.Headers;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Addresses.Dtos;
using UberSy.Application.Features.Addresses.Mappers;
using UberSy.Domain.Addresses;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Addresses.Commands.CreateAddress;

public sealed class CreateAddressCommandHandler(
    IApplicationDbContext context,
    IUser user,
    ILogger<CreateAddressCommandHandler> logger)
    : IRequestHandler<CreateAddressCommand, Result<AddressDto>>
{
    private readonly IUser _user = user;
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<CreateAddressCommandHandler> _logger = logger;

    public async Task<Result<AddressDto>> Handle(CreateAddressCommand request, CancellationToken cancellation)
    {
        if (_user.Id is null) return ApplicationErrors.Unauthorized;

        var userId = Guid.Parse(_user.Id);

        var boundary = new Point(request.Longitude, request.Latitude) { SRID = 4326 };

        var isFirstAddress = !await _context.TbAddresses
            .AsNoTracking()
            .AnyAsync(a => a.UserId == userId, cancellation);

        var createResult = TbAddress.Create(
            id: Guid.NewGuid(),
            userId: userId,
            street: request.Street,
            city: request.City,
            boundary: boundary,
            label: request.Label,
            floor: request.Floor,
            phone: request.Phone);

        if (createResult.IsError)
        {
            _logger.LogError("Failed To Create Address: {Error}", createResult.TopError.Description);
            return createResult.Errors;
        }

        var address = createResult.Value;

        if (isFirstAddress)
        {
            address.MarkAsDefault();
        }

        _context.TbAddresses.Add(address);

        await _context.SaveChangesAsync(cancellation);

        _logger.LogInformation("Address '{AddressId}' Created For User '{UserId}'.", address.Id, userId);

        return address.ToDto();
    }
}