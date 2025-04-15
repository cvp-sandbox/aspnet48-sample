using Microsoft.AspNetCore.Mvc;

namespace EventRegistrationSystemCore.ViewComponents;

public class LegacyUrlViewComponent : ViewComponent
{
    private readonly IConfiguration _configuration;

    public LegacyUrlViewComponent(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IViewComponentResult Invoke(string route)
    {
        // Return the relative route to be handled by YARP
        return Content(route);  // Just return the relative URL path
    }

    //public IViewComponentResult Invoke(string route)
    //{
    //    // Access the legacy app URL from the correct section
    //    var legacyAppUrl = _configuration["ReverseProxy:Clusters:legacyCluster:Destinations:destination1:Address"];
    //    // Combine the base URL with the route using Uri
    //    var baseUri = new Uri(legacyAppUrl);
    //    var combinedUri = new Uri(baseUri, route);  // Combines base URL with relative path

    //    Console.WriteLine($"Legacy URL: {combinedUri}");

    //    return Content(combinedUri.ToString());
    //}
}

