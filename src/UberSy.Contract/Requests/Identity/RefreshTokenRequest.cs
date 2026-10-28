namespace UberSy.Contract.Requests.Identity;

public sealed record RefreshTokenRequest(string RefreshToken, string ExpiredAccessToken);
