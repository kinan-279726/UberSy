using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Respawn;
using Testcontainers.MsSql;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.SubcutaneousTests.Common.Fakes;
using UberSy.Infrastructure.Data;
using UberSy.Infrastructure.Identity;
using UberSy.Test.Common.Security;
using Xunit;

namespace UberSy.Application.SubcutaneousTests.Common;

public class WebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private Respawner _respawner = default!;

    public TestCurrentUser TestUser => Services.GetRequiredService<TestCurrentUser>();

    public IMediator CreateMediator()
        => Services.CreateScope().ServiceProvider.GetRequiredService<IMediator>();

    public IApplicationDbContext CreateAppDbContext()
       => Services.CreateScope().ServiceProvider.GetRequiredService<IApplicationDbContext>();

    public async Task ResetDatabaseAsync()
    {
        TestUser.Reset();
        await using var connection = new SqlConnection(_dbContainer.GetConnectionString());
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", "Server=placeholder;Database=placeholder");
        builder.UseSetting("Verification:UseFakeSenders", "true");
        builder.UseSetting("JwtSettings:Issuer", "UberSy.Tests");
        builder.UseSetting("JwtSettings:Audience", "UberSy.Tests");
        builder.UseSetting("JwtSettings:Secret", "test-secret-key-at-least-32-characters-long!!");
        builder.UseSetting("Routing:OsrmBaseUrl", "http://localhost:5000");

        foreach (var (key, value) in TestSettings.All)
            builder.UseSetting(key, value);

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IHostedService>();
            services.RemoveAll<IUser>();

            services.RemoveAll<IRoutingService>();
            services.AddSingleton<FakeRoutingService>();
            services.AddSingleton<IRoutingService>(sp => sp.GetRequiredService<FakeRoutingService>());

            // SignalR notifiers replaced by one recording fake
            services.RemoveAll<IUserNotifier>();
            services.RemoveAll<IDriverNotifier>();
            services.RemoveAll<IRestaurantNotifier>();
            services.AddSingleton<FakeNotifier>();
            services.AddSingleton<IUserNotifier>(sp => sp.GetRequiredService<FakeNotifier>());
            services.AddSingleton<IDriverNotifier>(sp => sp.GetRequiredService<FakeNotifier>());
            services.AddSingleton<IRestaurantNotifier>(sp => sp.GetRequiredService<FakeNotifier>());

            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetService<ISaveChangesInterceptor>()!);
                options.UseSqlServer(
                    _dbContainer.GetConnectionString(),
                    sqlOptions => sqlOptions.UseNetTopologySuite());
            });

            services.AddSingleton<TestCurrentUser>();
            services.AddSingleton<IUser>(sp => sp.GetRequiredService<TestCurrentUser>());
        });
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        await _dbContainer.StartAsync();

        await using var connection = new SqlConnection(_dbContainer.GetConnectionString());
        await connection.OpenAsync();

        _ = Services;

        using (var scope = Services.CreateScope())
        {
            // Create schema + seed roles (idempotent, safe even if Program already ran it)
            var initialiser = ActivatorUtilities.CreateInstance<ApplicationDbContextInitialiser>(scope.ServiceProvider);
            await initialiser.InitialiseAsync();
            await initialiser.SeedAsync();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed test users once; Respawn ignores AspNetUsers so they survive resets
            var users = new[]
            {
                TestUsers.Admin, TestUsers.Customer, TestUsers.Customer2, TestUsers.Owner1, TestUsers.Owner2,
                TestUsers.Driver1, TestUsers.Driver2, TestUsers.Driver3 , TestUsers.Agent
            };

            foreach (var u in users)
                if (!await db.Users.AnyAsync(x => x.Id == u.Id))
                {
                    //db.Users.Add(u);
                    //u.SecurityStamp ??= Guid.NewGuid().ToString();

                    var result = await userManager.CreateAsync(u, "Password123!");

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to create test user {u.Email}: {errors}");
                    }
                }

            await db.SaveChangesAsync();
        }

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            TablesToIgnore = [
                new Respawn.Graph.Table("__EFMigrationsHistory"),
            new Respawn.Graph.Table("AspNetRoles"),
            new Respawn.Graph.Table("AspNetUsers"),
            new Respawn.Graph.Table("AspNetUserRoles"),
        ],
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
