using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Restaurants.Images;

public static class RestaurantImageErrors
{
    public static Error RestaurantIdRequired =>
        Error.Validation("Restaurant_Id_Required", "Restaurant Id Is Required");

    public static Error ImageUrlRequired =>
        Error.Validation("Image_Url_Required", "Image Url Is Required");

    public static Error ImageUrlTooLong =>
        Error.Validation("Image_Url_Too_Long", "Image Url Cannot Exceed 1000 Characters");
}
