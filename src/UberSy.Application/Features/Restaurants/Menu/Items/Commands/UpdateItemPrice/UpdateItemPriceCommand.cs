using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.UpdateItemPrice;

public sealed record UpdateItemPriceCommand(Guid ItemId, decimal Price)
    : IRequest<Result<Updated>>;
