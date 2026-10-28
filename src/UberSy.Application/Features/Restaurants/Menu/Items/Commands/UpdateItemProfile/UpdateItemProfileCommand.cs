using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.UpdateItemProfile;

public sealed record UpdateItemProfileCommand(
    Guid ItemId,
    string NameAr,
    string NameEn,
    string DescriptionAr,
    string DescriptionEn,
    string ImageUrl)
    : IRequest<Result<Updated>>;
