using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Addresses.Commands.UpdateAddressDetails;
public sealed record UpdateAddressDetailsCommand(
    Guid AddressId, string Street, string City, double Latitude, double Longitude,
    string? Label, string? Floor, string? Phone) : IRequest<Result<Updated>>;
