using NetTopologySuite.Geometries;
using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;

namespace UberSy.Domain.ServiceZones;

public sealed class TbServiceZone : Entity
{
    public string NameAr { get; private set; } = null!;

    public string NameEn { get; private set; } = null!;

    public Geometry Boundary { get; private set; } = null!;

    public string City { get; private set; } = null!;

    public bool IsActive { get; private set; }

    private TbServiceZone()
    { }

    private TbServiceZone(Guid id, string nameAr, string nameEn, Geometry boundary, string city)
        :base(id)
    {
        NameAr = nameAr;
        NameEn = nameEn;
        Boundary = boundary;
        City = city;
        IsActive = true;
    }

    public static Result<TbServiceZone> Create(Guid id, string nameAr, string nameEn, Geometry boundary, string city)
    {
        var validation = Validate(nameAr, nameEn, boundary, city);

        if (validation.IsError)
        {
            return validation.Errors;
        }

        return new TbServiceZone(id, nameAr.Trim(), nameEn.Trim(), boundary, city.Trim());
    }

    public Result<Updated> Update(string nameAr, string nameEn, string city)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
        {
            return ServiceZoneErrors.NameArRequired;
        }

        if (nameAr.Length > 100)
        {
            return ServiceZoneErrors.NameArTooLong;
        }

        if (string.IsNullOrWhiteSpace(nameEn))
        {
            return ServiceZoneErrors.NameEnRequired;
        }

        if (nameEn.Length > 100)
        {
            return ServiceZoneErrors.NameEnTooLong;
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return ServiceZoneErrors.CityRequired;
        }

        if (city.Length > 100)
        {
            return ServiceZoneErrors.CityTooLong;
        }

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        City = city.Trim();

        return Result.Updated;
    }

    public Result<Updated> Activate()
    {
        IsActive = true;

        return Result.Updated;
    }

    public Result<Updated> Deactivate()
    {
        IsActive = false;

        return Result.Updated;
    }

    private static Result<Success> Validate(string nameAr, string nameEn, Geometry boundary, string city)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
        {
            return ServiceZoneErrors.NameArRequired;
        }

        if (nameAr.Length > 100)
        {
            return ServiceZoneErrors.NameArTooLong;
        }

        if (string.IsNullOrWhiteSpace(nameEn))
        {
            return ServiceZoneErrors.NameEnRequired;
        }

        if (nameEn.Length > 100)
        {
            return ServiceZoneErrors.NameEnTooLong;
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return ServiceZoneErrors.CityRequired;
        }

        if (city.Length > 100)
        {
            return ServiceZoneErrors.CityTooLong;
        }

        var boundaryResult = ValidateBoundary(boundary);
        if (boundaryResult.IsError)
        {
            return boundaryResult.Errors;
        }

        return Result.Success;
    }

    private static Result<Success> ValidateBoundary(Geometry boundary)
    {
        if (boundary is null)
        {
            return ServiceZoneErrors.BoundaryRequired;
        }

        if (boundary is not (Polygon or MultiPolygon))
        {
            return ServiceZoneErrors.BoundaryMustBePolygon;
        }

        if (!boundary.IsValid)
        {
            return boundary.IsSimple
                ? ServiceZoneErrors.BoundaryNotClosed
                : ServiceZoneErrors.BoundarySelfIntersects;
        }

        return Result.Success;
    }

}
