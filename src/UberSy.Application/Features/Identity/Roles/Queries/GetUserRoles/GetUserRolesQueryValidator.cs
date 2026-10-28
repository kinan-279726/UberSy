using FluentValidation;

namespace UberSy.Application.Features.Identity.Roles.Queries.GetUserRoles;

public sealed class GetUserRolesQueryValidator : AbstractValidator<GetUserRolesQuery>
{
    public GetUserRolesQueryValidator() => RuleFor(x => x.UserId).NotEmpty();
}
