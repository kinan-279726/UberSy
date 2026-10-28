namespace UberSy.Contract.Requests.Identity;

public sealed record RegisterRequest(string Email, string Password, string NameAr, string NameEn, string Phone);
