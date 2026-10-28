using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Commands.OutForDelivery;

public sealed class OutForDelivaryCommandHandler(IApplicationDbContext context,IOwnershipValidator ownership , ILogger<OutForDelivaryCommandHandler> logger)
    : IRequestHandler<OutForDelivaryCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly ILogger<OutForDelivaryCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(OutForDelivaryCommand request, CancellationToken cancellationToken)
    {
        var driverResult = await _ownership.IsDriver();
        if(driverResult.IsError)
        {
            return driverResult.Errors;
        }


       var order = await _context.TbFoodOrders.AnyAsync(o => o.DriverId == driverResult.Value, cancellationToken);

        if (!order) return ApplicationErrors.ServiceRequestNotFound;

        var servic = await _context.TbServiceRequests.FirstOrDefaultAsync(s => s.Id == request.ServiceRequestId, cancellationToken);

        if(servic is null) return ApplicationErrors.ServiceRequestNotFound;

        var serviceResult = servic.MarkAsOutForDelivery();

        if (serviceResult.IsError) return serviceResult.Errors;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("service with id {request.ServiceRequestId} out for delivary", request.ServiceRequestId);

        return Result.Updated;
    }
}