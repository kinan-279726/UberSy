using MediatR;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Identity.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Admin.Queries.Users;

public sealed record GetUsersQuery(string? Search, string? Role, bool? IsBanned, int PageNumber = 1, int PageSize = 20)
    : IRequest<Result<PagedList<AdminUserListItemDto>>>;
