using FluentValidation;

namespace UberSy.Application.Features.Identity.Roles.Queries.GetUsersByRole;

public sealed class GetUsersByRoleQueryValidator : AbstractValidator<GetUsersByRoleQuery>
{
    public GetUsersByRoleQueryValidator()
    {
        RuleFor(x => x.Role).NotEmpty().Must(r => Domain.Identity.Roles.Normalize(r) is not null);
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
