using MediatR;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Queries.MyWallet;

public sealed record GetMyWalletQuery : IRequest<Result<DriverWalletDto>>;
