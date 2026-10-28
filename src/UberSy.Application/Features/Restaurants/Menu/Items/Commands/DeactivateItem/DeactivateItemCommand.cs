using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.DeactivateItem;

public sealed record DeactivateItemCommand(Guid ItemId) : IRequest<Result<Updated>>;
