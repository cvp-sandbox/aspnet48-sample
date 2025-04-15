using EventManagement.Api.Common.DependencyInjection;
using EventManagement.Api.Configuration;
using EventManagement.Api.Features.Events;
using Scalar.AspNetCore;

// Load environment variables from .env file
DotEnvReader.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (telemetry, health checks, etc.)
builder.AddServiceDefaults();

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



var app = builder.Build();


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();

    app.MapOpenApi();              // Exposes the OpenAPI JSON at /openapi/v1.json
    app.MapScalarApiReference();   // Exposes the Scalar UI at /scalar/v1
}


app.UseHttpsRedirection();

app.UseHeaderAuthentication(); 

// Map endpoints
app.MapEventsEndpoints();

// Map service defaults (health checks, etc.)
app.MapDefaultEndpoints();

app.Run();

// Make Program class accessible to tests
public partial class Program { }