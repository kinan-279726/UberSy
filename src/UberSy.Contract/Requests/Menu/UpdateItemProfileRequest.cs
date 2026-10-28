namespace UberSy.Contract.Requests.Menu;

public sealed record UpdateItemProfileRequest(
    string NameAr, string NameEn, string DescriptionAr, string DescriptionEn, string ImageUrl);

