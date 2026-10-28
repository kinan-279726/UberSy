using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.Categories.Dtos;
using UberSy.Application.Features.Restaurants.Menu.Categories.Mappers;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Quiries.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler(IApplicationDbContext context) : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _context.TbCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.CategoryId , cancellationToken);

        if (category is null) return ApplicationErrors.CategoryNotFound;

        return category.ToDto();
    }
}