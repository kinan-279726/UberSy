namespace UberSy.Domain.Identity;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Owner = "Owner";
    public const string Customer = "Customer";
    public const string Driver = "Driver";
    public const string Agent = "Agent";

    public static readonly string[] All = [Admin, Owner, Customer, Driver, Agent];

    public static string? Normalize(string? role) =>
    All.FirstOrDefault(r => string.Equals(r, role?.Trim(), StringComparison.OrdinalIgnoreCase));

}
