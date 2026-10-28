using MediatR;
using UberSy.Application.Features.Restaurants.Menu.Categories.Dtos;
using UberSy.Domain.Common.Results;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    Guid RestaurantId,
    Guid? PrinterId,
    string NameAr,
    string NameEn,
    byte SortOrder)
    : IRequest<Result<CategoryDto>>;