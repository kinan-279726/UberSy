using MediatR;
using System.Collections;
using System.Linq.Expressions;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.Items.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemsByCategory;

public sealed record GetItemsByCategoryQuery(Guid CategoryId) : ICachedQuery<Result<List<ItemDto>>>
{
    public string CacheKey => $"items{CategoryId}";

    public string[] Tags => [$"items{CategoryId}"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(1);
}