using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace EventRegistrationSystem.Utils
{
    public static class DotEnvReader
    {
        public static Dictionary<string, string> Load(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
            if (!File.Exists(filePath))
                throw new System.IO.FileNotFoundException($"No .env file found at {filePath}. Skipping environment variable loading.");

            var envVariables = new Dictionary<string, string>();

            if (!File.Exists(filePath))
            {
                return envVariables;
            }

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    continue;
                }

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                envVariables[key] = value;
            }

            return envVariables;
        }

    }
}
