using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Queries.GenerateTokens;

public sealed record GenerateTokenQuery(
string Email,
string Password) : IRequest<Result<TokenResponse>>;
