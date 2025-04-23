using EventManagement.Api.Common.Identity;
using EventManagement.Api.Features.Users.Login;
using EventManagement.Api.Features.Users.LogOff;
using EventManagement.Api.Features.Users.Register;
using EventManagement.Api.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagement.Api.Features.Users;

public static class UsersModule
{

    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        // Register password hasher
        services.AddSingleton<LegacyCompatiblePasswordHasher<IdentityUser>>();

        // Configure PasswordHasher options
        services.Configure<PasswordHasherOptions>(options =>
        {
            // This helps with compatibility for older hashes
            options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2;
        });

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        // Register handlers
        services.AddScoped<LoginHandler>();
        services.AddScoped<RegisterHandler>();
        services.AddScoped<LogOffHandler>();

        return services;
    }

    public static WebApplication MapUsersEndpoints(this WebApplication app)
    {
        // Map endpoints
        app.MapLoginEndpoint();
        app.MapRegisterEndpoint();
        app.MapLogOffEndpoint();

        return app;
    }
}

