using MediatR;
using UberSy.Domain.Common.Results;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.AssignAddOnToItem;

public sealed record AssignAddOnToItemCommand(Guid ItemId, Guid AddOnId) : IRequest<Result<Updated>>;
