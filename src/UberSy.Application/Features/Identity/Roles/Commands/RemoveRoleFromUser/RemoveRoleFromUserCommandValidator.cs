using FluentValidation;

namespace UberSy.Application.Features.Identity.Roles.Commands.RemoveRoleFromUser;

public sealed class RemoveRoleFromUserCommandValidator : AbstractValidator<RemoveRoleFromUserCommand>
{
    public RemoveRoleFromUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty()
            .Must(r => Domain.Identity.Roles.Normalize(r) is not null).WithMessage("Role Is Not Valid.");
    }
}
