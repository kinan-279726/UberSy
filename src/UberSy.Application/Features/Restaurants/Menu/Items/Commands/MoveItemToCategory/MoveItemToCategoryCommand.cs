using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.MoveItemToCategory;

public sealed record MoveItemToCategoryCommand(Guid ItemId, Guid NewCategoryId) : IRequest<Result<Updated>>;
