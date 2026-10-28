using UberSy.Domain.Common.Results;

namespace UberSy.Application.Common.Interfaces;

public interface IDriverDispatchService
{
    Task<Result<Updated>> DispatchNextBatchAsync(Guid serviceRequestId, CancellationToken cancellation = default);

}
