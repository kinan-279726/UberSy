namespace UberSy.Contract.Requests.Menu;

public sealed record CreateCategoryRequest(Guid? PrinterId, string NameAr, string NameEn, byte SortOrder);

