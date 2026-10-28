using FluentValidation;

public sealed class GetAddressByIdQueryValidator : AbstractValidator<GetAddressByIdQuery>
{
    public GetAddressByIdQueryValidator() => RuleFor(x => x.AddressId).NotEmpty();
}
