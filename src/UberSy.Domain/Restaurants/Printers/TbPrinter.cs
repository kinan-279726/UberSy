using UberSy.Domain.Common;
using UberSy.Domain.Restaurants.Categories;

namespace UberSy.Domain.Restaurants.Printers;

public partial class TbPrinter : Entity
{
    public Guid RestaurantId { get; set; }

    public byte Type { get; set; }

    public string? Name { get; set; }

    public string ConnectionInfo { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual TbRestaurant Restaurant { get; set; } = null!;

    public virtual ICollection<TbCategory> TbCategories { get; set; } = new List<TbCategory>();
}
