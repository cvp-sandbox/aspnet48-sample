namespace EventRegistrationSystemCore.Utils;

public static class DotEnvReader
{
    public static Dictionary<string, string> Load(string filePath)
    {
        var envVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!File.Exists(filePath)) return envVariables;

        foreach (var line in File.ReadAllLines(filePath))
        {
            var lineSpan = line.AsSpan().Trim();

            // Skip comments and empty lines
            if (lineSpan.IsEmpty || lineSpan[0] == '#') continue;

            // Split on first '=' only
            var splitIndex = lineSpan.IndexOf('=');
            if (splitIndex < 0) continue;

            var key = lineSpan[..splitIndex].Trim().ToString();
            var value = lineSpan[(splitIndex + 1)..].Trim().ToString();

            // Remove surrounding quotes if present
            if (value.Length > 1 &&
                ((value[0] == '"' && value[^1] == '"') ||
                 (value[0] == '\'' && value[^1] == '\'')))
            {
                value = value[1..^1];
            }

            if (!string.IsNullOrWhiteSpace(key))
            {
                envVariables[key] = value;
            }
        }

        return envVariables;
    }
}
