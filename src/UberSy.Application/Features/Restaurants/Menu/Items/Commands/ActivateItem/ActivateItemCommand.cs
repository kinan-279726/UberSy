using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.ActivateItem;

public sealed record ActivateItemCommand(Guid ItemId) : IRequest<Result<Updated>>;
