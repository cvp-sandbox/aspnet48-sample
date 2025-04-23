using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Cryptography;

namespace EventManagement.Api.Common.Identity;

/// <summary>
    /// Custom password hasher that supports verifying password hashes from Microsoft.AspNet.Identity.Owin
    /// while also supporting the newer ASP.NET Core Identity hashing algorithm.
    /// </summary>
    public class LegacyCompatiblePasswordHasher<TUser> : PasswordHasher<TUser> where TUser : class
    {
        private readonly PasswordHasherCompatibilityMode _compatibilityMode;

        public LegacyCompatiblePasswordHasher(Microsoft.Extensions.Options.IOptions<PasswordHasherOptions> optionsAccessor = null)
        {
            var options = optionsAccessor?.Value ?? new PasswordHasherOptions();
            _compatibilityMode = options.CompatibilityMode;
        }

        public override string HashPassword(TUser user, string password)
        {
            // Always use the new ASP.NET Core Identity hashing algorithm for new passwords
            return base.HashPassword(user, password);
        }

        public override PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));
            if (providedPassword == null) throw new ArgumentNullException(nameof(providedPassword));

            // First try to verify using ASP.NET Core Identity format (for migrated users)
            var result = base.VerifyHashedPassword(user, hashedPassword, providedPassword);
            
            // If verification succeeds or if it's definitely an ASP.NET Core Identity hash format,
            // return the result
            if (result != PasswordVerificationResult.Failed || hashedPassword.StartsWith("AQ"))
            {
                return result;
            }

            // If we get here, the hash might be in Legacy ASP.NET Identity format (from the Owin implementation)
            // Check if it's in the format from Microsoft.AspNet.Identity.Owin
            if (IsLegacyHash(hashedPassword))
            {
                bool verified = VerifyLegacyHash(hashedPassword, providedPassword);
                
                if (verified)
                {
                    // Password is correct, but was stored in the legacy format
                    // Return SuccessRehashNeeded to update to the new format
                    return PasswordVerificationResult.SuccessRehashNeeded;
                }
            }

            // If we get here, verification failed
            return PasswordVerificationResult.Failed;
        }

        private bool IsLegacyHash(string hashedPassword)
        {
            // Legacy ASP.NET Identity hashes typically have a specific format
            // For example, they often contain a '|' character to separate parts
            // Modify this based on your specific legacy hash format
            return hashedPassword.Contains("|");
        }

        private bool VerifyLegacyHash(string hashedPassword, string providedPassword)
        {
            // Split the hash into its components
            // Typical format is: "hashedPassword|salt|format"
            string[] parts = hashedPassword.Split('|');
            
            if (parts.Length < 2)
            {
                return false;
            }

            string storedHash = parts[0];
            string salt = parts[1];
            string format = parts.Length > 2 ? parts[2] : "SHA1";

            // Hash the provided password using the same algorithm
            string computedHash = HashPasswordWithFormat(providedPassword, salt, format);
            
            // Compare the computed hash with the stored hash
            return string.Equals(storedHash, computedHash, StringComparison.OrdinalIgnoreCase);
        }

        private string HashPasswordWithFormat(string password, string salt, string format)
        {
            // Combine password and salt
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(password);
            byte[] saltBytes = Convert.FromBase64String(salt);
            
            byte[] combinedBytes = new byte[saltBytes.Length + bytes.Length];
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(bytes, 0, combinedBytes, saltBytes.Length, bytes.Length);

            // Hash with the specified format
            byte[] hashBytes;
            switch (format.ToUpper())
            {
                case "SHA1":
                    using (var sha1 = SHA1.Create())
                    {
                        hashBytes = sha1.ComputeHash(combinedBytes);
                    }
                    break;
                    
                case "HMACSHA256":
                    using (var hmac = new HMACSHA256(saltBytes))
                    {
                        hashBytes = hmac.ComputeHash(bytes);
                    }
                    break;
                    
                case "PBKDF2":
                    // For PBKDF2, we need the iteration count which should be in the next part
                    // This is just a basic implementation - you might need to adjust based on your specific format
                    int iterCount = 1000; // Default iteration count
                    if (format.Contains(":"))
                    {
                        string[] formatParts = format.Split(':');
                        if (formatParts.Length > 1 && int.TryParse(formatParts[1], out int parsedIterCount))
                        {
                            iterCount = parsedIterCount;
                        }
                    }
                    
                    using (var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes, iterCount, HashAlgorithmName.SHA1))
                    {
                        hashBytes = deriveBytes.GetBytes(20); // SHA1 produces 20 bytes
                    }
                    break;
                    
                default:
                    throw new NotSupportedException($"Hash format {format} is not supported.");
            }

            // Return Base64 encoded hash
            return Convert.ToBase64String(hashBytes);
        }
    }
