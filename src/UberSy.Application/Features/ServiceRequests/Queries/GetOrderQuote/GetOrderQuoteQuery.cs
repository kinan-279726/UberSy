using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using UberSy.Application.Features.ServiceRequests.Commands.PlaceFoodOrder;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetOrderQuote;

public sealed record GetOrderQuoteQuery(
    Guid RestaurantId,
    Guid AddressId,
    List<PlaceFoodOrderItem> Items) : IRequest<Result<OrderQuoteDto>>;
