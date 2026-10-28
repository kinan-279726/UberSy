using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants;

namespace UberSy.Domain.Tags;

public sealed class TbTag : Entity
{
    public string NameAr { get; private set; } = null!;

    public string NameEn { get; private set; } = null!;

    private readonly List<TbRestaurant> _restaurants = [];
    public IReadOnlyCollection<TbRestaurant> Restaurants => _restaurants.AsReadOnly();

    private TbTag()
    { }

    private TbTag(Guid id, string nameAr, string nameEn)
        : base(id)
    {
        NameAr = nameAr;
        NameEn = nameEn;
    }

    public static Result<TbTag> Create(Guid id, string nameAr, string nameEn)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
        {
            return TagErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(nameEn))
        {
            return TagErrors.NameRequired;
        }

        return new TbTag(id, nameAr.Trim(), nameEn.Trim());
    }

    public Result<Updated> Update(string nameAr, string nameEn)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
        {
            return TagErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(nameEn))
        {
            return TagErrors.NameRequired;
        }

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();

        return Result.Updated;
    }
}
