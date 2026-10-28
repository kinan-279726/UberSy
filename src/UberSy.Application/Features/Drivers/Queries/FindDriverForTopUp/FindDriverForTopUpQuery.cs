using MediatR;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Queries.FindDriverForTopUp;

public sealed record FindDriverForTopUpQuery(string Search) : IRequest<Result<AgentDriverLookupDto>>;
