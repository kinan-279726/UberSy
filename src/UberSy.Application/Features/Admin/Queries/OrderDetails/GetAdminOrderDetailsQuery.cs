using MediatR;
using UberSy.Application.Features.Admin.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Admin.Queries.OrderDetails;

public sealed record GetAdminOrderDetailsQuery(Guid ServiceRequestId) : IRequest<Result<AdminOrderDetailsDto>>;
