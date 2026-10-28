using FluentValidation;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetRestaurantStats;

public sealed class GetRestaurantStatsQueryValidator : AbstractValidator<GetRestaurantStatsQuery>
{
    public GetRestaurantStatsQueryValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x).Must(x => !x.From.HasValue || !x.To.HasValue || x.From <= x.To)
            .WithMessage("From must be before or equal to To.");
        RuleFor(x => x).Must(x => !x.From.HasValue || !x.To.HasValue
                || x.To.Value.DayNumber - x.From.Value.DayNumber <= 92)
            .WithMessage("The range cannot exceed 93 days.");
    }
}