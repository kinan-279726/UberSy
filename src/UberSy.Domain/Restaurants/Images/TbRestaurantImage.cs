using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Restaurants.Images;

public sealed class TbRestaurantImage : AuditableEntity
{
    public Guid RestaurantId { get; private set; }
    public string ImageUrl { get; private set; } = null!;
    public byte SortOrder { get; private set; }

    // مرجعية فقط
    public TbRestaurant Restaurant { get; private set; } = null!;

    private TbRestaurantImage()
    { }

    private TbRestaurantImage(Guid id, Guid restaurantId, string imageUrl, byte sortOrder)
        : base(id)
    {
        RestaurantId = restaurantId;
        ImageUrl = imageUrl;
        SortOrder = sortOrder;
    }

    public static Result<TbRestaurantImage> Create(Guid id, Guid restaurantId, string imageUrl, byte sortOrder)
    {
        if (restaurantId == Guid.Empty) return RestaurantImageErrors.RestaurantIdRequired;
        if (string.IsNullOrWhiteSpace(imageUrl)) return RestaurantImageErrors.ImageUrlRequired;
        if (imageUrl.Length > 1000) return RestaurantImageErrors.ImageUrlTooLong;

        return new TbRestaurantImage(id, restaurantId, imageUrl.Trim(), sortOrder);
    }

    public Result<Updated> UpdateImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl)) return RestaurantImageErrors.ImageUrlRequired;
        if (imageUrl.Length > 1000) return RestaurantImageErrors.ImageUrlTooLong;

        ImageUrl = imageUrl.Trim();

        return Result.Updated;
    }

    public Result<Updated> UpdateSortOrder(byte sortOrder)
    {
        SortOrder = sortOrder;
        return Result.Updated;
    }
}
