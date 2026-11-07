using MediatR;
using System.Diagnostics.CodeAnalysis;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Commands.TopUpWallet;

public sealed record TopUpWalletCommand(decimal Amount , Guid DriverId) : IRequest<Result<Updated>>;
