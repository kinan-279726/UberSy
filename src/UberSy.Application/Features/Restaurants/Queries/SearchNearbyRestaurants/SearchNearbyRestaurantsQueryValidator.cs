using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Queries.SearchNearbyRestaurants;

public sealed class SearchNearbyRestaurantsQueryValidator : AbstractValidator<SearchNearbyRestaurantsQuery>
{
    public SearchNearbyRestaurantsQueryValidator()
    {
        RuleFor(x => x.RadiusKm).GreaterThan(0).LessThanOrEqualTo(50);

        RuleFor(x => x.Latitude)
           .NotEmpty()
           .WithMessage("Latitude Is Required")
           .InclusiveBetween(-90, 90)
           .WithMessage("Latitude Must Be Between -90 And 90");

        RuleFor(x => x.Longitude)
            .NotEmpty()
            .WithMessage("Longitude Is Required")
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude Must Be Between -180 And 180");

        RuleFor(x => x)
            .Must(x => !(x.Latitude == 0 && x.Longitude == 0))
            .WithMessage("Coordinates Cannot Be (0, 0) — Null Island")
            .WithName("Coordinates");

        RuleFor(x => x)
            .Must(x => !double.IsNaN(x.Latitude) && !double.IsInfinity(x.Latitude))
            .WithMessage("Latitude Must Be A Valid Number")
            .WithName("Latitude");

        RuleFor(x => x)
            .Must(x => !double.IsNaN(x.Longitude) && !double.IsInfinity(x.Longitude))
            .WithMessage("Longitude Must Be A Valid Number")
            .WithName("Longitude");
    }
}
