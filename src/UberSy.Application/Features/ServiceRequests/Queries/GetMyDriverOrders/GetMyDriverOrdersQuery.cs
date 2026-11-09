using FluentValidation;
using MediatR;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetMyDriverOrders;

//driver
public sealed record GetMyDriverOrdersQuery(ServiceRequestStatus? Status, int PageNumber = 1, int PageSize = 20)
    : IRequest<Result<PagedList<DriverOrderListItemDto>>>;

public sealed class GetMyDriverOrdersQueryValidator : AbstractValidator<GetMyDriverOrdersQuery>
{
    public GetMyDriverOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}
