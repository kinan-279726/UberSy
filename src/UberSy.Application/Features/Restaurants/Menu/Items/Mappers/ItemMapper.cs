using UberSy.Application.Features.Restaurants.Menu.AddOns.Mappers;
using UberSy.Application.Features.Restaurants.Menu.Items.Dtos;
using UberSy.Domain.Restaurants.Items;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Mappers;

public static class ItemMapper
{
    public static ItemDto ToDto(this TbItem entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new ItemDto
        {
            Id = entity.Id,
            RestaurantId = entity.RestaurantId,
            CategoryId = entity.CategoryId,
            NameAr = entity.NameAr,
            NameEn = entity.NameEn,
            DescriptionAr = entity.DescriptionAr,
            DescriptionEn = entity.DescriptionEn,
            Price = entity.Price,
            ImageUrl = entity.ImageUrl,
            Calories = entity.Calories,
            IsActive = entity.IsActive,
            SortOrder = entity.SortOrder,
            AddOns = entity.AddOns.ToDtos()
        };
    }

    public static List<ItemDto> ToDtos(this IEnumerable<TbItem> entities)
    {
        return [.. entities.Select(e => e.ToDto())];
    }
}
