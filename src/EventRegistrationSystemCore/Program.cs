using EventRegistrationSystemCore.Identity;
using EventRegistrationSystemCore.Models;
using EventRegistrationSystemCore.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Data;
using System.Security.Claims;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Load .env file BEFORE other configuration
var envFilePath = Path.Combine(builder.Environment.ContentRootPath, ".env");
Dictionary<string, string> envVars = DotEnvReader.Load(envFilePath);

// Add .env variables to configuration
builder.Configuration
    .AddInMemoryCollection(envVars
    .Select(kvp => new KeyValuePair<string, string?>(kvp.Key, kvp.Value)));

// Continue with normal configuration setup
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();


// Add services to the container.


// Configure YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transforms =>
    {
        //transforms.AddRequestTransform(context =>
        //{
        //    // Forward headers from response to the proxied request
        //    foreach (var headerName in new[] { "X-UserId", "X-UserName", "X-UserEmail", "X-UserRoles" })
        //    {
        //        if (context.HttpContext.Response.Headers.TryGetValue(headerName, out var headerValue))
        //        {
        //            context.ProxyRequest.Headers.Remove(headerName);
        //            context.ProxyRequest.Headers.Add(headerName, headerValue.ToArray());
        //            Console.WriteLine($"Forwarding response header {headerName}: {string.Join(", ", headerValue)}");
        //        }
        //        // Also check request headers
        //        else if (context.HttpContext.Request.Headers.TryGetValue(headerName, out headerValue))
        //        {
        //            context.ProxyRequest.Headers.Remove(headerName);
        //            context.ProxyRequest.Headers.Add(headerName, headerValue.ToArray());
        //            Console.WriteLine($"Forwarding request header {headerName}: {string.Join(", ", headerValue)}");
        //        }
        //        // Finally, check if User is authenticated
        //        else if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        //        {
        //            string value = null;
        //            switch (headerName)
        //            {
        //                case "X-UserId":
        //                    value = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //                    break;
        //                case "X-UserName":
        //                    value = context.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        //                    break;
        //                case "X-UserEmail":
        //                    value = context.HttpContext.User.FindFirstValue(ClaimTypes.Email);
        //                    break;
        //                case "X-UserRoles":
        //                    value = string.Join(",", context.HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value));
        //                    break;
        //            }
        //            if (!string.IsNullOrEmpty(value))
        //            {
        //                context.ProxyRequest.Headers.Remove(headerName);
        //                context.ProxyRequest.Headers.Add(headerName, value);
        //                Console.WriteLine($"Setting header from claims {headerName}: {value}");
        //            }
        //        }
        //    }
        //    return ValueTask.CompletedTask;
        //});
        //// Forward cookies including authentication cookies
        //transforms.AddRequestTransform(context =>
        //{
        //    if (context.HttpContext.Request.Cookies.Count > 0)
        //    {
        //        string cookieValue = string.Join("; ", context.HttpContext.Request.Cookies.Select(c => $"{c.Key}={c.Value}"));
        //        context.ProxyRequest.Headers.Remove("Cookie");
        //        context.ProxyRequest.Headers.Add("Cookie", cookieValue);
        //    }
        //    return ValueTask.CompletedTask;
        //});
    });

// Configure SQLite connection
// Initialize SQLite once at startup
Batteries.Init();
builder.Services.AddScoped<IDbConnection>(sp =>
{
    string? dbFile = builder.Configuration["DatabasePath"];
    if (string.IsNullOrEmpty(dbFile))
    {
        throw new InvalidOperationException("DatabasePath configuration is missing or empty.");
    }
    string connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source={DbFile};";

    var cs = connectionString.Replace("{DbFile}", dbFile);

    return new SqliteConnection(cs);

});

// Configure Identity
// Add required Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddDefaultTokenProviders();

// Register SQLiteUserStore as implementation of relevant interfaces
builder.Services.AddScoped<IUserStore<ApplicationUser>, SQLiteUserStore>();
builder.Services.AddScoped<IUserPasswordStore<ApplicationUser>, SQLiteUserStore>();
builder.Services.AddScoped<IUserEmailStore<ApplicationUser>, SQLiteUserStore>();
builder.Services.AddScoped<IUserLockoutStore<ApplicationUser>, SQLiteUserStore>();
builder.Services.AddScoped<IUserTwoFactorStore<ApplicationUser>, SQLiteUserStore>();

builder.Services.AddScoped<IRoleStore<IdentityRole>, SQLiteRoleStore>();


// Add authentication and identity services
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{

    options.Cookie.Name = GetAuthCookieName(); // Use a consistent cookie name
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.Cookie.Path = "/";

    // Add events to debug the cookie creation
    options.Events = new CookieAuthenticationEvents
    {
        OnSignedIn = context =>
        {
            Console.WriteLine("User signed in");
            return Task.CompletedTask;
        },
        OnSigningOut = context =>
        {
            Console.WriteLine("User signing out");
            return Task.CompletedTask;
        },
        OnRedirectToLogin = context =>
        {
            Console.WriteLine("Redirecting to login");
            return Task.CompletedTask;
        },
        OnValidatePrincipal = context =>
        {
            Console.WriteLine("Cookie validating principal event");
            Console.WriteLine($"Authentication scheme: {context.Scheme.Name}");
            Console.WriteLine($"Cookie name: {context.Options.Cookie.Name}");
            Console.WriteLine($"Has cookie: {context.HttpContext.Request.Cookies.ContainsKey(context.Options.Cookie.Name)}");
            return Task.CompletedTask;
        }
    };
});




// Add the custom SQLite user store
builder.Services.AddScoped<SQLiteUserStore>();

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    // Basic policy for authenticated users
    options.AddPolicy("RequireAuthenticated", policy =>
        policy.RequireAuthenticatedUser());

    // Admin policy
    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireRole("Admin"));

    // Organizer policy
    options.AddPolicy("RequireOrganizer", policy =>
        policy.RequireRole("Organizer", "Admin"));
});

builder.Services.AddControllersWithViews();

//=======================================================================================

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();

// Add this middleware after UseAuthentication but before UseAuthorization
app.Use(async (context, next) =>
{
    // If the user is not authenticated but we have an auth cookie
    if (context.User.Identity?.IsAuthenticated != true &&
        context.Request.Cookies.ContainsKey(GetAuthCookieName()))
    {
        // Manually authenticate the user
        var authResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (authResult.Succeeded)
        {
            // Replace the current user principal with the authenticated one
            context.User = authResult.Principal;
            Console.WriteLine($"Manually authenticated user: {context.User.Identity?.Name}");
        }
        else
        {
            Console.WriteLine($"Manual authentication failed: {authResult.Failure?.Message}");
        }
    }

    await next();
});
app.UseAuthorization();

app.MapStaticAssets();

// Add a specific endpoint for Account/Login that maps directly to the controller
app.MapControllerRoute(
    name: "login",
    pattern: "Account/Login/{*pathInfo}",
    defaults: new { controller = "Account", action = "Login" });

// Map other controllers
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();

//// Then add your custom middleware for header handling if needed
//app.Use(async (context, next) =>
//{
//    Console.WriteLine($"Request path: {context.Request.Path}");
//    // Check if auth cookie exists
//    if (context.Request.Cookies.TryGetValue(GetAuthCookieName(), out var cookieValue))
//    {
//        Console.WriteLine("Auth cookie found in request");
//        Console.WriteLine($"Cookie length: {cookieValue.Length}");
//        Console.WriteLine($"Cookie starts with: {cookieValue.Substring(0, Math.Min(10, cookieValue.Length))}");

//        // Try to manually validate the cookie
//        var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//        Console.WriteLine($"Manual authentication result: {result.Succeeded}");
//        if (!result.Succeeded)
//        {
//            Console.WriteLine($"Authentication failure reason: {result.Failure?.Message}");
//        }
//    }
//    else
//    {
//        Console.WriteLine("No auth cookie found");
//        Console.WriteLine($"Available cookies: {string.Join(", ", context.Request.Cookies.Keys)}");
//    }

//    // Check if user is authenticated
//    if (context.User.Identity?.IsAuthenticated == true)
//    {
//        Console.WriteLine($"User authenticated as: {context.User.Identity.Name}");
//        Console.WriteLine($"Authentication type: {context.User.Identity.AuthenticationType}");
//        Console.WriteLine($"Claims: {string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
//    }
//    else
//    {
//        Console.WriteLine("User not authenticated");
//    }

//    await next();
//});

// Map YARP as the LAST endpoint
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(async (context, next) =>
    {
        // Skip Account/Login routes
        if (context.Request.Path.StartsWithSegments("/Account/Login"))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        Console.WriteLine("Debug: All request headers before processing");
        foreach (var header in context.Request.Headers)
        {
            Console.WriteLine($"  {header.Key}: {header.Value}");
        }

        // If user is authenticated, add the headers
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = context.User.FindFirstValue(ClaimTypes.Name);
            var email = context.User.FindFirstValue(ClaimTypes.Email);
            var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);

            // Clear any existing headers to avoid duplication
            context.Request.Headers.Remove(AuthHeaders.UserId);
            context.Request.Headers.Remove(AuthHeaders.UserName);
            context.Request.Headers.Remove(AuthHeaders.UserEmail);
            context.Request.Headers.Remove(AuthHeaders.UserRoles);

            // Add headers from authenticated user
            if (!string.IsNullOrEmpty(userId))
                context.Request.Headers.Append(AuthHeaders.UserId, userId);
            if (!string.IsNullOrEmpty(userName))
                context.Request.Headers.Append(AuthHeaders.UserName, userName);
            if (!string.IsNullOrEmpty(email))
                context.Request.Headers.Append(AuthHeaders.UserEmail, email);
            if (roles.Any())
                context.Request.Headers.Append(AuthHeaders.UserRoles, string.Join(",", roles));

            Console.WriteLine($"Added auth headers for user: {userName}");
        }
        else
        {
            Console.WriteLine("No authenticated user found for header forwarding");
        }

        Console.WriteLine("Debug: All request headers after processing");
        foreach (var header in context.Request.Headers)
        {
            Console.WriteLine($"  {header.Key}: {header.Value}");
        }

        await next();
    });
});

app.Run();

static string GetAuthCookieName()
{
    return ".AspNet.ApplicationCookie";
}