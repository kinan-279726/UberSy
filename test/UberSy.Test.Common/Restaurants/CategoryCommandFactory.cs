using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.ActivateCategory;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.CreateCategory;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.DeactivateCategory;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.RemovePrinterFromCategory;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategoryProfile;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategorySortOrder;

namespace UberSy.Test.Common.Restaurants;

public static class CategoryCommandFactory
{
    // 1. Activate Category
    public static ActivateCategoryCommand CreateActivateCategoryCommand(
        Guid? categoryId = null)
    {
        return new ActivateCategoryCommand(
            categoryId ?? Guid.NewGuid());
    }

    // 2. Assign Printer To Category
    public static AssignPrinterToCategoryCommand CreateAssignPrinterToCategoryCommand(
        Guid? categoryId = null,
        Guid? printerId = null)
    {
        return new AssignPrinterToCategoryCommand(
            categoryId ?? Guid.NewGuid(),
            printerId ?? Guid.NewGuid());
    }

    // 3. Create Category
    public static CreateCategoryCommand CreateCreateCategoryCommand(
        Guid? restaurantId = null,
        Guid? printerId = null,
        string? nameAr = null,
        string? nameEn = null,
        byte? sortOrder = null)
    {
        return new CreateCategoryCommand(
            restaurantId ?? Guid.NewGuid(),
            printerId,
            nameAr ?? "المشاوي",
            nameEn ?? "Grills",
            sortOrder ?? 1);
    }

    // 4. Deactivate Category
    public static DeactivateCategoryCommand CreateDeactivateCategoryCommand(
        Guid? categoryId = null)
    {
        return new DeactivateCategoryCommand(
            categoryId ?? Guid.NewGuid());
    }

    // 5. Remove Printer From Category
    public static RemovePrinterFromCategoryCommand CreateRemovePrinterFromCategoryCommand(
        Guid? categoryId = null)
    {
        return new RemovePrinterFromCategoryCommand(
            categoryId ?? Guid.NewGuid());
    }

    // 6. Update Category Profile
    public static UpdateCategoryProfileCommand CreateUpdateCategoryProfileCommand(
        Guid? categoryId = null,
        string? nameAr = null,
        string? nameEn = null)
    {
        return new UpdateCategoryProfileCommand(
            categoryId ?? Guid.NewGuid(),
            nameAr ?? "المشاوي",
            nameEn ?? "Grills");
    }

    // 7. Update Category Sort Order
    public static UpdateCategorySortOrderCommand CreateUpdateCategorySortOrderCommand(
        Guid? categoryId = null,
        byte? sortOrder = null)
    {
        return new UpdateCategorySortOrderCommand(
            categoryId ?? Guid.NewGuid(),
            sortOrder ?? 1);
    }
}
