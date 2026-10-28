using MediatR;
using QuestPDF.Infrastructure;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateOperatingHours;

public sealed record UpdateOperatingHoursCommand(Guid RestaurantId, TimeOnly OpeningTime, TimeOnly ClosingTime) : IRequest<Result<Updated>>;
