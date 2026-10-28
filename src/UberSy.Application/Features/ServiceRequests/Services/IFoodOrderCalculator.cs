using UberSy.Application.Features.ServiceRequests.Commands.PlaceFoodOrder;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Services;

public interface IFoodOrderCalculator
{
    Task<Result<FoodOrderCalculation>> CalculateAsync(
        Guid customerId,
        Guid restaurantId,
        Guid addressId,
        IReadOnlyCollection<PlaceFoodOrderItem> items,
        CancellationToken cancellationToken);
}