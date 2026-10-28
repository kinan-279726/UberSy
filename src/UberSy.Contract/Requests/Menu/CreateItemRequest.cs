namespace UberSy.Contract.Requests.Menu;

public sealed record CreateItemRequest(
    Guid CategoryId, string NameAr, string NameEn,
    string DescriptionAr, string DescriptionEn,
    decimal Price, string ImageUrl, int? Calories, byte SortOrder);

