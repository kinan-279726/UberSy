using UberSy.Application.Features.Restaurants.Menu.Items.Commands.ActivateItem;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.AssignAddOnToItem;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.DeactivateItem;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.MoveItemToCategory;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.RemoveAddOnFromItem;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.UpdateItemPrice;
using UberSy.Application.Features.Restaurants.Menu.Items.Commands.UpdateItemProfile;
using UberSy.Application.Features.Restaurants.Menu.Items.requests.CreateItem;

namespace UberSy.Test.Common.Restaurants;

public static class ItemCommandFactory
{
    // 1. Activate Item
    public static ActivateItemCommand CreateActivateItemCommand(
        Guid? itemId = null)
    {
        return new ActivateItemCommand(
            itemId ?? Guid.NewGuid());
    }

    // 2. Assign AddOn To Item
    public static AssignAddOnToItemCommand CreateAssignAddOnToItemCommand(
        Guid? itemId = null,
        Guid? addOnId = null)
    {
        return new AssignAddOnToItemCommand(
            itemId ?? Guid.NewGuid(),
            addOnId ?? Guid.NewGuid());
    }

    // 3. Create Item
    public static CreateItemCommand CreateCreateItemCommand(
        Guid? restaurantId = null,
        Guid? categoryId = null,
        string? nameAr = null,
        string? nameEn = null,
        string? descriptionAr = null,
        string? descriptionEn = null,
        decimal? price = null,
        string? imageUrl = null,
        int? calories = null,
        byte? sortOrder = null)
    {
        return new CreateItemCommand(
            restaurantId ?? Guid.NewGuid(),
            categoryId ?? Guid.NewGuid(),
            nameAr ?? "شاورما دجاج",
            nameEn ?? "Chicken Shawarma",
            descriptionAr ?? "شاورما دجاج طازجة مع صوص الثوم",
            descriptionEn ?? "Fresh chicken shawarma with garlic sauce",
            price ?? 25m,
            imageUrl ?? "https://cdn.ubersy.com/items/default.jpg",
            calories ?? 350,
            sortOrder ?? 1);
    }

    // 4. Deactivate Item
    public static DeactivateItemCommand CreateDeactivateItemCommand(
        Guid? itemId = null)
    {
        return new DeactivateItemCommand(
            itemId ?? Guid.NewGuid());
    }

    // 5. Move Item To Category
    public static MoveItemToCategoryCommand CreateMoveItemToCategoryCommand(
        Guid? itemId = null,
        Guid? newCategoryId = null)
    {
        return new MoveItemToCategoryCommand(
            itemId ?? Guid.NewGuid(),
            newCategoryId ?? Guid.NewGuid());
    }

    // 6. Remove AddOn From Item
    public static RemoveAddOnFromItemCommand CreateRemoveAddOnFromItemCommand(
        Guid? itemId = null,
        Guid? addOnId = null)
    {
        return new RemoveAddOnFromItemCommand(
            itemId ?? Guid.NewGuid(),
            addOnId ?? Guid.NewGuid());
    }

    // 7. Update Item Price
    public static UpdateItemPriceCommand CreateUpdateItemPriceCommand(
        Guid? itemId = null,
        decimal? price = null)
    {
        return new UpdateItemPriceCommand(
            itemId ?? Guid.NewGuid(),
            price ?? 30m);
    }

    // 8. Update Item Profile
    public static UpdateItemProfileCommand CreateUpdateItemProfileCommand(
        Guid? itemId = null,
        string? nameAr = null,
        string? nameEn = null,
        string? descriptionAr = null,
        string? descriptionEn = null,
        string? imageUrl = null)
    {
        return new UpdateItemProfileCommand(
            itemId ?? Guid.NewGuid(),
            nameAr ?? "شاورما دجاج",
            nameEn ?? "Chicken Shawarma",
            descriptionAr ?? "شاورما دجاج طازجة مع صوص الثوم",
            descriptionEn ?? "Fresh chicken shawarma with garlic sauce",
            imageUrl ?? "https://cdn.ubersy.com/items/default.jpg");
    }
}
