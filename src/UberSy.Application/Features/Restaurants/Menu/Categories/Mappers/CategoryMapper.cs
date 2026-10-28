using UberSy.Application.Features.Restaurants.Menu.Categories.Dtos;
using UberSy.Domain.Restaurants.Categories;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Mappers;

public static class CategoryMapper
{
    public static CategoryDto ToDto(this TbCategory entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new CategoryDto
        {
            Id = entity.Id,
            RestaurantId = entity.RestaurantId,
            PrinterId = entity.PrinterId,
            NameAr = entity.NameAr,
            NameEn = entity.NameEn,
            IsActive = entity.IsActive,
            SortOrder = entity.SortOrder,
        };
    }

    public static List<CategoryDto> ToDtos(this List<TbCategory> entities)
    {
        return [.. entities.Select(x => ToDto(x)).ToList()];
    }
}
