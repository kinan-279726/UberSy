using UberSy.Domain.Common.Results;
using UberSy.Domain.Reviews;

namespace UberSy.Test.Common.Review;

public static class ReviewFactory
{
    public static Result<TbReview> CreateReview(
        Guid? id = null,
        Guid? customerId = null,
        Guid? serviceRequestId = null,
        Guid? restaurantId = null,
        Guid? driverId = null,
        byte? rating = null,
        string? note = null)
    {
        // يجب تمرير واحد فقط من restaurantId أو driverId
        // الافتراضي: مراجعة مطعم
        var restId = restaurantId ?? (driverId is null ? Guid.NewGuid() : (Guid?)null);
        var drvId = driverId;

        return TbReview.Create(
            id ?? Guid.NewGuid(),
            customerId ?? Guid.NewGuid(),
            serviceRequestId ?? Guid.NewGuid(),
            restId,
            drvId,
            rating ?? 5,
            note);
    }
}
