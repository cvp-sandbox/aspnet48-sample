namespace EventRegistrationSystemCore.Utils;

public static class AuthHeaders
{
    public const string UserId = "X-UserId";
    public const string UserName = "X-UserName";
    public const string UserEmail = "X-UserEmail";
    public const string UserRoles = "X-UserRoles";

    public static string? GetCustomHeader(this HttpRequest request, string headerName)
    {
        return request.Headers[headerName];
    }

    // Add method to check if all authentication headers exist
    public static bool HasAuthHeaders(this HttpRequest request)
    {
        return request.Headers.ContainsKey(UserId) &&
               request.Headers.ContainsKey(UserName);
    }

    // Add method to log all auth headers
    public static void LogAuthHeaders(this HttpRequest request)
    {
        Console.WriteLine($"Auth Headers: UserId={request.Headers[UserId]}, " +
                          $"UserName={request.Headers[UserName]}, " +
                          $"UserRoles={request.Headers[UserRoles]}");
    }
}
