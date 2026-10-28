using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Commands.RemovePrinterFromCategory;

public sealed record RemovePrinterFromCategoryCommand(Guid CategoryId) : IRequest<Result<Updated>>;