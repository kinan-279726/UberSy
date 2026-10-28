using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.RemoveAddOnFromItem;

public sealed record RemoveAddOnFromItemCommand(Guid ItemId, Guid AddOnId) : IRequest<Result<Updated>>;
