using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UberSy.Application.Common.Interfaces;
using UberSy.Infrastructure.BackgroundServices;
using UberSy.Infrastructure.Data;
using UberSy.Infrastructure.Data.Interceptors;
using UberSy.Infrastructure.Identity;
using UberSy.Infrastructure.Notifications;
using UberSy.Infrastructure.Services;
using UberSy.Infrastructure.Settings;

namespace UberSy.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        services.Configure<SmsSettings>(configuration.GetSection("Sms"));

        if (configuration.GetValue<bool>("Verification:UseFakeSenders"))
        {
            services.AddScoped<LoggingMessageSender>();
            services.AddScoped<IEmailSender>(sp => sp.GetRequiredService<LoggingMessageSender>());
            services.AddScoped<ISmsSender>(sp => sp.GetRequiredService<LoggingMessageSender>());
        }
        else
        {
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddScoped<ISmsSender, TwilioSmsSender>();
        }

        services.AddScoped<IVerificationService, VerificationService>();

        services.AddSingleton(TimeProvider.System);

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetService<ISaveChangesInterceptor>()!);
            options.UseSqlServer(connectionString, sqlOptions => sqlOptions.UseNetTopologySuite());
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)),
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    // يُقبل التوكن من الرابط لمسارات الـ Hubs فقط
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                },
            };
        });

        PolicyExtensions.AddCustomPolicies(services);

        services
        .AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 1;
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddTransient<IIdentityService, IdentityService>();

        services.AddHybridCache(options => options.DefaultEntryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromSeconds(30),
        });

        services.AddHttpClient<IRoutingService, OsrmRoutingService>(client =>
        {
            client.BaseAddress = new Uri(configuration["Routing:OsrmBaseUrl"]!);
            client.Timeout = TimeSpan.FromSeconds(3);
        });

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
        });

        services.AddDataProtection()
            .PersistKeysToDbContext<ApplicationDbContext>();

        services.AddSignalR();

        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddHostedService<OrderResponseTimeoutService>();
        services.AddScoped<IOwnershipValidator, OwnershipValidator>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IRestaurantNotifier, SignalRRestaurantNotifier>();

        services.AddScoped<IDriverNotifier, SignalRDriverNotifier>();
        services.AddScoped<IUserNotifier, SignalRUserNotifier>();
        services.AddHostedService<DriverOfferTimeoutService>();
        services.AddScoped<IUserRoleService, UserRoleService>();

        return services;
    }
}
