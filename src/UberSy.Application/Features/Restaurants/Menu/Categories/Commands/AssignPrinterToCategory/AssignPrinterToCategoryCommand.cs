using MediatR;
using UberSy.Domain.Common.Results;

public sealed record AssignPrinterToCategoryCommand(Guid CategoryId, Guid PrinterId) : IRequest<Result<Updated>>;
