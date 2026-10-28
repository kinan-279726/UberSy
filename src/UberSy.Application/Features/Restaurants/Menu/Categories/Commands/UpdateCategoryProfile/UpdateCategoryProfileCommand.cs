using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategoryProfile;

public sealed record UpdateCategoryProfileCommand(Guid CategoryId, string NameAr, string NameEn)
    : IRequest<Result<Updated>>;
