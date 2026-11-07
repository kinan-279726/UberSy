using UberSy.Application.Features.Addresses.Commands.DeleateAddress;
using UberSy.Application.Features.Addresses.Commands.UpdateAddressDetails;

namespace UberSy.Application.SubcutaneousTests.Common.Factories;

public static class AddressCommandFactory
{
    // 1. Delete Address
    public static DeleteAddressCommand CreateDeleteAddressCommand(
        Guid? addressId = null)
    {
        return new DeleteAddressCommand(
            addressId ?? Guid.NewGuid());
    }

    // 2. Mark Address As Default
    public static MarkAddressAsDefaultCommand CreateMarkAddressAsDefaultCommand(
        Guid? addressId = null)
    {
        return new MarkAddressAsDefaultCommand(
            addressId ?? Guid.NewGuid());
    }

    // 3. Update Address Details
    public static UpdateAddressDetailsCommand CreateUpdateAddressDetailsCommand(
    Guid? addressId = null,
    string? street = null,
    string? city = null,
    double? latitude = null,
    double? longitude = null,
    string? label = null,
    string? floor = null,
    string? phone = null)
    {
        return new UpdateAddressDetailsCommand(
            addressId ?? Guid.NewGuid(),
            street ?? "Updated Street",
            city ?? "Aleppo",
            latitude ?? 36.2021,
            longitude ?? 37.1343,
            label ?? "Work",
            floor ?? "5",
            phone ?? "+963912345679");
    }
}

