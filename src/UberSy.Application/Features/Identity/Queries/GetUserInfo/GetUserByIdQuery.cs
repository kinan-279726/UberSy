using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Queries.GetUserInfo;

public sealed record GetUserByIdQuery(string? UserId) : IRequest<Result<ApplicationUserDto>>;
