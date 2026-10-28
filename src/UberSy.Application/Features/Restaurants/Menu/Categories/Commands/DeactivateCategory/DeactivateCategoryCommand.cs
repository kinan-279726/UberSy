using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Commands.DeactivateCategory;

public sealed record DeactivateCategoryCommand(Guid CategoryId) : IRequest<Result<Updated>>;
