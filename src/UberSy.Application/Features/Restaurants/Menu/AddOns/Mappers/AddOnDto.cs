using UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;
using UberSy.Domain.Restaurants.AddOns;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Mappers;

public static class AddOnMapper
{
    public static AddOnDto ToDto(this TbAddOn entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new AddOnDto
        {
            Id = entity.Id,
            RestaurantId = entity.RestaurantId,
            NameAr = entity.NameAr,
            NameEn = entity.NameEn,
            Price = entity.Price,
            IsActive = entity.IsActive,
        };
    }

    public static List<AddOnDto> ToDtos(this IEnumerable<TbAddOn> entities)
    {
        return [.. entities.Select(e => e.ToDto())];
    }
}
