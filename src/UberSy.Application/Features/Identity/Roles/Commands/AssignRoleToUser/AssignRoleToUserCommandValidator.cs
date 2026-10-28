using FluentValidation;

namespace UberSy.Application.Features.Identity.Roles.Commands.AssignRoleToUser;

public sealed class AssignRoleToUserCommandValidator : AbstractValidator<AssignRoleToUserCommand>
{
    public AssignRoleToUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty()
            .Must(r => Domain.Identity.Roles.Normalize(r) is not null).WithMessage("Role Is Not Valid.");
    }
}