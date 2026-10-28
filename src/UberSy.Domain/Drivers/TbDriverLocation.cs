using NetTopologySuite.Geometries;
using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Drivers;

public sealed class TbDriverLocation : Entity
{
    public Guid DriverId { get; private set; }

    public Point Boundary { get; private set; } = null!;
    
    public DateTime UpdatedAt { get; private set; }

    public TbDriver Driver { get; private set; } = null!;

    private TbDriverLocation()
    { }

    private TbDriverLocation(Guid id, Guid driverId, Point boundary)
        : base(id)
    {
        DriverId = driverId;
        Boundary = boundary;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Result<TbDriverLocation> Create(Guid id, Guid driverId, Point boundary)
    {
        if (driverId == Guid.Empty) return DriverErrors.UserIdRequired;
        if (boundary is null || !boundary.IsValid) return DriverLocationErrors.BoundaryInvalid;

        return new TbDriverLocation(id, driverId, boundary);
    }

    public Result<Updated> UpdateLocation(Point boundary)
    {
        if (boundary is null || !boundary.IsValid) return DriverLocationErrors.BoundaryInvalid;

        Boundary = boundary;
        UpdatedAt = DateTime.UtcNow;

        return Result.Updated;
    }
}
