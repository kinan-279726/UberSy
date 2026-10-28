using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Domain.Restaurants;

namespace UberSy.Application.Features.Restaurants.Mappers;

public static class RestaurantMapper
{
    public static RestaurantDto ToDto(this TbRestaurant entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new RestaurantDto
        {
            Id = entity.Id,
            NameAr = entity.NameAr,
            NameEn = entity.NameEn,
            Phone = entity.Phone,
            IsApproved = entity.IsApproved,
            IsOpenNow = entity.IsOpenNow,
            Rating = entity.Rating,
        };
    }
    public static List<RestaurantDto> ToDtos(this IEnumerable<TbRestaurant> entities)
    {
        return [.. entities.Select(e => e.ToDto())];
    }
}
