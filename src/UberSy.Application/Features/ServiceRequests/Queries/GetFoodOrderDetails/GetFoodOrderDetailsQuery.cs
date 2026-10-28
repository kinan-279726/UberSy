using MediatR;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetFoodOrderDetails;

public sealed record GetFoodOrderDetailsQuery(Guid ServiceRequestId) : IRequest<Result<FoodOrderDetailsDto>>;
