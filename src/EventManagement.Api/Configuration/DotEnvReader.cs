namespace EventManagement.Api.Configuration;

public static class DotEnvReader
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
            return;
            
        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split('=', 2);
            if (parts.Length != 2)
                continue;
                
            var key = parts[0].Trim();
            var value = parts[1].Trim();
            
            if (string.IsNullOrEmpty(key))
                continue;
                
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}