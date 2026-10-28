using MediatR;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Queries.MyActiveOrder;

public sealed record GetMyActiveOrderQuery : IRequest<Result<ActiveDeliveryDto>>;