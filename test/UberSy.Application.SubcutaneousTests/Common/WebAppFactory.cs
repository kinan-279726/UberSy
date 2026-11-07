using MediatR;
using Microsoft.AspNetCore.Hosting;
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
using UberSy.Api.Services;
using UberSy.Application.Common.Interfaces;
using UberSy.Infrastructure.Data;
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

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IHostedService>();
            services.RemoveAll<IUser>();

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
