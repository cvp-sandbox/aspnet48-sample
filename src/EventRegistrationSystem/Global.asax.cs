using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EventRegistrationSystem.Utils;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System;

namespace EventRegistrationSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LoadEnvSettings();

            AreaRegistration.RegisterAllAreas();

            // Register dependencies FIRST
            DependencyConfig.RegisterDependencies();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void LoadEnvSettings()
        {
            var envFilePath = Server.MapPath("~/.env");

            var envVariables = DotEnvReader.Load(envFilePath);

            foreach (var envVariable in envVariables)
            {
                var key = envVariable.Key;
                var value = envVariable.Value;

                if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    ConfigurationManager.AppSettings[key] = value;

                }
                else
                {
                    ConfigurationManager.AppSettings.Add(key, value);
                }
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                // Check if we have auth headers from YARP
                string userId = context.Request.GetCustomHeader(AuthHeaders.UserId);
                string userName = context.Request.GetCustomHeader(AuthHeaders.UserName);
                string userEmail = context.Request.GetCustomHeader(AuthHeaders.UserEmail);
                string userRoles = context.Request.GetCustomHeader(AuthHeaders.UserRoles);

                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userName))
                {
                    // Create claims
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, userId),
                            new Claim(ClaimTypes.Name, userName),
                            // Add this claim which is required for anti-forgery token support
                            new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                                      "ASP.NET Identity", // Or some other provider name
                                      ClaimValueTypes.String)
                        };

                    if (!string.IsNullOrEmpty(userEmail))
                        claims.Add(new Claim(ClaimTypes.Email, userEmail));

                    // Add roles
                    if (!string.IsNullOrEmpty(userRoles))
                    {
                        foreach (var role in userRoles.Split(','))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
                        }
                    }

                    // Create identity and principal
                    var identity = new ClaimsIdentity(claims, ".AspNet.ApplicationCookie");
                    context.User = new ClaimsPrincipal(identity);
                }
            }
        }

    }
}

public static class AuthHeaders
{
    public const string UserId = "X-UserId";
    public const string UserName = "X-UserName";
    public const string UserEmail = "X-UserEmail";
    public const string UserRoles = "X-UserRoles";

    public static string GetCustomHeader(this HttpRequest request, string headerName)
    {
        return request.Headers[headerName];
    }

    // Add method to check if all authentication headers exist
    public static bool HasAuthHeaders(this HttpRequest request)
    {
        return request.Headers.AllKeys.Contains(UserId) &&
               request.Headers.AllKeys.Contains(UserName);
    }

    // Add method to log all auth headers
    public static void LogAuthHeaders(this HttpRequest request)
    {
        Console.WriteLine($"Auth Headers: UserId={request.Headers[UserId]}, " +
                          $"UserName={request.Headers[UserName]}, " +
                          $"UserRoles={request.Headers[UserRoles]}");
    }
}
