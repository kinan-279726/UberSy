using UberSy.Domain.Common.Results;

namespace UberSy.Application.Common.Interfaces;

public interface IOwnershipValidator
{
    bool IsAnonymous();

    Task<Result<Guid>> IsUser();

    bool IsInRole(string role);

    Task<Result<Guid>> IsDriver();

    Result<bool> EnsureIsAManager(CancellationToken cancellationToken = default);

    Task<Result<bool>> EnsureIsOwnerByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default);

    Task<Result<bool>> EnsureIsOwnerByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);

    Task<Result<Guid>> EnsureCanEditAddress(Guid addressId, CancellationToken cancellationToken = default);

}
