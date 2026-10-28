using MediatR;
using UberSy.Application.Features.Admin.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Admin.Queries.AdminDashboard;

public sealed record GetAdminDashboardQuery(DateOnly? From, DateOnly? To) : IRequest<Result<AdminDashboardDto>>;
