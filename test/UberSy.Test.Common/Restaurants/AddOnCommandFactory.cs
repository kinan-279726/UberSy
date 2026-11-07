using UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.ActivateAddOn;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.CreateAddOn;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.DeactivateAddOn;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.UpdateAddOn;

namespace UberSy.Test.Common.Restaurants;

public static class AddOnCommandFactory
{
    // 1. Activate AddOn
    public static ActivateAddOnCommand CreateActivateAddOnCommand(
        Guid? addOnId = null)
    {
        return new ActivateAddOnCommand(
            addOnId ?? Guid.NewGuid());
    }

    // 2. Create AddOn
    public static CreateAddOnCommand CreateCreateAddOnCommand(
        Guid? restaurantId = null,
        string? nameAr = null,
        string? nameEn = null,
        decimal? price = null)
    {
        return new CreateAddOnCommand(
            restaurantId ?? Guid.NewGuid(),
            nameAr ?? "جبنة إضافية",
            nameEn ?? "Extra Cheese",
            price ?? 2.5m);
    }

    // 3. Deactivate AddOn
    public static DeactivateAddOnCommand CreateDeactivateAddOnCommand(
        Guid? addOnId = null)
    {
        return new DeactivateAddOnCommand(
            addOnId ?? Guid.NewGuid());
    }

    // 4. Update AddOn
    public static UpdateAddOnCommand CreateUpdateAddOnCommand(
        Guid? addOnId = null,
        string? nameAr = null,
        string? nameEn = null,
        decimal? price = null)
    {
        return new UpdateAddOnCommand(
            addOnId ?? Guid.NewGuid(),
            nameAr ?? "جبنة إضافية",
            nameEn ?? "Extra Cheese",
            price ?? 2.5m);
    }
}
