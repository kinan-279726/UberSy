using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategorySortOrder;

public sealed record UpdateCategorySortOrderCommand(Guid CategoryId, byte SortOrder) : IRequest<Result<Updated>>;
