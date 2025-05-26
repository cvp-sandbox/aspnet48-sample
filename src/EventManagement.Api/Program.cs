using EventManagement.Api.Common.DependencyInjection;
using EventManagement.Api.Common.Identity;
using EventManagement.Api.Configuration;
using EventManagement.Api.Features.Events;
using EventManagement.Api.Features.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

// Load environment variables from .env file
DotEnvReader.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (telemetry, health checks, etc.)
builder.AddServiceDefaults();

// Configure CORS to allow requests from the SPA
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpaOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:44372")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure JWT Authentication
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<JwtTokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = jwtSettings.GetSymmetricSecurityKey()
    };
});

builder.Services.AddAuthorization();

// Add configuration from environment variables after loading from appsettings
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Required for endpoint discovery  
builder.Services.AddEndpointsApiExplorer();

// Required for Microsoft's OpenAPI generation  
builder.Services.AddOpenApi();

//builder.Services.AddSwaggerGen();     //requires Swashbuckle.AspNetCore NuGet package

// Add SQLite connection
builder.Services.AddSqliteConnection(builder.Configuration);

// Add feature modules
builder.Services.AddEventsModule();
builder.Services.AddUsersModule();



var app = builder.Build();


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();

    app.MapOpenApi();              // Exposes the OpenAPI JSON at /openapi/v1.json
    app.MapScalarApiReference();   // Exposes the Scalar UI at /scalar/v1

    //rediirect root GET request to Scalar UI
    app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}


app.UseHttpsRedirection();

// Enable CORS with the policy
app.UseCors("AllowSpaOrigin");


// Enable Authentication & Authorization
// This will process JWT tokens from the Authorization header
app.UseAuthentication();
app.UseAuthorization();

// For backward compatibility, also use header authentication
// This will only authenticate if JWT authentication didn't already succeed
// It checks for X-User/X-Username and X-Role headers
app.UseHeaderAuthentication();



// Map endpoints
app.MapEventsEndpoints();
app.MapUsersEndpoints();

// Map service defaults (health checks, etc.)
app.MapDefaultEndpoints();

app.Run();

// Make Program class accessible to tests
public partial class Program { }
