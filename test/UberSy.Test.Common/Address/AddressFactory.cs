using NetTopologySuite.Geometries;
using UberSy.Domain.Addresses;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.SubcutaneousTests.Common.Factories;

public static class AddressFactory
{
    public static Result<TbAddress> CreateAddress(
        Guid? id = null,
        Guid? userId = null,
        string? street = null,
        string? city = null,
        Point? boundary = null,
        string? label = null,
        string? floor = null,
        string? phone = null)
    {
        return TbAddress.Create(
            id ?? Guid.NewGuid(),
            userId ?? Guid.NewGuid(),
            street ?? "King Faisal Street",
            city ?? "Damascus",
            boundary ?? CreatePoint(),
            label ?? "Home",
            floor ?? "1",
            phone ?? "+963912345678");
    }

    public static TbAddress CreateValidAddress(
        Guid? id = null,
        Guid? userId = null,
        string? street = null,
        string? city = null,
        Point? boundary = null,
        string? label = null,
        string? floor = null,
        string? phone = null)
    {
        var result = CreateAddress(id, userId, street, city, boundary, label, floor, phone);

        if (result.IsError)
        {
            throw new InvalidOperationException(
                $"AddressFactory produced invalid data: {result.TopError.Code} - {result.TopError.Description}");
        }

        return result.Value;
    }

    private static Point CreatePoint(double lon = 36.2765, double lat = 33.5138)
        => new(lon, lat) { SRID = 4326 };
}
