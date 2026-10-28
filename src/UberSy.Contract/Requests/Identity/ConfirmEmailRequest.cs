namespace UberSy.Contract.Requests.Identity;

public sealed record ConfirmEmailRequest(string Email, string Code);
