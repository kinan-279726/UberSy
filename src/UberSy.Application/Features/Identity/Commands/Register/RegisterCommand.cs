using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Commands.Register;

public sealed record RegisterCommand(string Email,
    string Password,
    string NameAr,
    string NameEn,
    string Phone)
    : IRequest<Result<ApplicationUserDto>>;
