using MediatR;
using UberSy.Application.Features.Restaurants.Menu.Categories.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Quiries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid CategoryId) : IRequest<Result<CategoryDto>>;
