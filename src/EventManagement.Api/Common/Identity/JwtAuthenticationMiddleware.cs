using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace EventManagement.Api.Common.Identity;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtSettings _jwtSettings;

    public JwtAuthenticationMiddleware(RequestDelegate next, JwtSettings jwtSettings)
    {
        _next = next;
        _jwtSettings = jwtSettings;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            try
            {
                // Configure token validation parameters
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = _jwtSettings.GetSymmetricSecurityKey()
                };

                // Create token handler
                var tokenHandler = new JwtSecurityTokenHandler();
                
                // Validate token and extract principal
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                
                // Set the user on the HttpContext
                context.User = principal;
            }
            catch
            {
                // Token validation failed, continue to next middleware
            }
        }

        // Continue processing the request
        await _next(context);
    }
}

// Extension method to add the middleware to the pipeline
public static class JwtAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtAuthenticationMiddleware>();
    }
}
