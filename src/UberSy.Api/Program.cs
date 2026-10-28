using Scalar.AspNetCore;
using Serilog;
using UberSy.Api;
using UberSy.Application;
using UberSy.Infrastructure;
using UberSy.Infrastructure.Data;
using UberSy.Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddMediatR(options =>
//{
//    options.RegisterServicesFromAssemblies(typeof(IAssemblyMarker).Assembly);
//});

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "UberSy API V1");

        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.EnableFilter();
    });

    app.MapScalarApiReference();

    await app.InitialiseDatabaseAsync();
}

app.UseCoreMiddlewares(builder.Configuration);

app.MapControllers();

//app.UseAntiforgery();

app.MapStaticAssets();

app.UseStaticFiles();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapHub<RestaurantHub>("/hubs/restaurant");
app.MapHub<DriverHub>("/hubs/driver");
app.MapHub<UserHub>("/hubs/user");

app.Run();
