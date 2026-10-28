using MediatR;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Admin.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers.Enums;

namespace UberSy.Application.Features.Admin.Queries.GetDrivers;

public sealed record GetDriversQuery(
    bool? IsApproved,
    DriverStatus? Status,
    string? Search,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PagedList<AdminDriverListItemDto>>>;
