using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Commands.ActivateCategory;

public sealed record ActivateCategoryCommand(Guid CategoryId) : IRequest<Result<Updated>>;
