using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.DeactivateAddOn;

public sealed record DeactivateAddOnCommand(Guid AddOnId) : IRequest<Result<Updated>>;
