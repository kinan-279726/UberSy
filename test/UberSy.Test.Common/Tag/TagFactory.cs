using UberSy.Domain.Common.Results;
using UberSy.Domain.Tags;

namespace UberSy.Test.Common.Tag;

public static class TagFactory
{
    public static Result<TbTag> CreateTag(
        Guid? id = null,
        string? nameAr = null,
        string? nameEn = null)
    {
        return TbTag.Create(
            id ?? Guid.NewGuid(),
            nameAr ?? "مأكولات سريعة",
            nameEn ?? "Fast Food");
    }
}
