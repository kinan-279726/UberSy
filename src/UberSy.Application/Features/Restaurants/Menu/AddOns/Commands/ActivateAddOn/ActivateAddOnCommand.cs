using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.ActivateAddOn;

public sealed record ActivateAddOnCommand(Guid AddOnId) : IRequest<Result<Updated>>;
