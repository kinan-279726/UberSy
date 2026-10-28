using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.UpdateAddOn;

public sealed record class UpdateAddOnCommand(Guid AddOnId, string NameAr, string NameEn, decimal Price)
    : IRequest<Result<Updated>>;
