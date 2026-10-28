using MediatR;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers.Wallet.Enums;

namespace UberSy.Application.Features.Drivers.Queries.MyWalletTransactions;

public sealed record GetMyWalletTransactionsQuery(WalletTransactionType? Type, int PageNumber = 1, int PageSize = 20)
    : IRequest<Result<PagedList<WalletTransactionDto>>>;
