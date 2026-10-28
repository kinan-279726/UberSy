using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UberSy.Domain.Identity;
using UberSy.Infrastructure.Identity;

namespace UberSy.Infrastructure.Data;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;

    private readonly ApplicationDbContext _context;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<ApplicationRole> _roleManager;

    private readonly IConfiguration _configuration;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
    ApplicationDbContext context, UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IConfiguration configuration)
    {
        this._logger = logger;
        this._context = context;
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._configuration = configuration;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        foreach (var roleName in Roles.All)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole(roleName);
                await _roleManager.CreateAsync(role);
            }
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("do not forget to move admin email and password to user secrets key");
        Console.ResetColor();

        var email = _configuration["AdminSeed:Email"];
        var password = _configuration["AdminSeed:Password"];

        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
        {
            if (await _userManager.FindByEmailAsync(email) is null)
            {
                var admin = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    NameAr = "مدير النظام",
                    NameEn = "System Admin",
                };

                var created = await _userManager.CreateAsync(admin, password);

                if (created.Succeeded)
                {
                    await _userManager.AddToRolesAsync(admin, [Roles.Admin, Roles.Customer]);
                }
                else
                {
                    _logger.LogError("Failed To create admin: {Errors}",string.Join(", ", created.Errors.Select(e => e.Description)));
                }
            }
        }
        await _context.SaveChangesAsync();
    }
}