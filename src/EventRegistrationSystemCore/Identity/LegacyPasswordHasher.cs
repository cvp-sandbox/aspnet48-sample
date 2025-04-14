using EventRegistrationSystemCore.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;


namespace EventRegistrationSystemCore.Identity;

public class LegacyPasswordHasher : IPasswordHasher<ApplicationUser>
{
    // ASP.NET Identity 2.2.4 used PBKDF2 with HMAC-SHA1
    private const int PBKDF2IterCount = 1000; // Default in Identity 2.2.4
    private const int PBKDF2SubkeyLength = 256 / 8; // 256 bits
    private const int SaltSize = 128 / 8; // 128 bits

    public string HashPassword(ApplicationUser user, string password)
    {
        if (password == null)
            throw new ArgumentNullException(nameof(password));

        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        byte[] subkey = GenerateSaltedHash(password, salt, PBKDF2IterCount, PBKDF2SubkeyLength);

        var outputBytes = new byte[1 + SaltSize + PBKDF2SubkeyLength];
        outputBytes[0] = 0x00; // Format marker
        Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
        Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, PBKDF2SubkeyLength);
        return Convert.ToBase64String(outputBytes);
    }

    public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
    {
        if (hashedPassword == null)
            throw new ArgumentNullException(nameof(hashedPassword));
        if (providedPassword == null)
            throw new ArgumentNullException(nameof(providedPassword));

        byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

        // Verify format marker
        if (decodedHashedPassword.Length == 0 || decodedHashedPassword[0] != 0x00)
        {
            return PasswordVerificationResult.Failed;
        }

        if (decodedHashedPassword.Length != 1 + SaltSize + PBKDF2SubkeyLength)
        {
            return PasswordVerificationResult.Failed;
        }

        byte[] salt = new byte[SaltSize];
        Buffer.BlockCopy(decodedHashedPassword, 1, salt, 0, SaltSize);

        byte[] expectedSubkey = new byte[PBKDF2SubkeyLength];
        Buffer.BlockCopy(decodedHashedPassword, 1 + SaltSize, expectedSubkey, 0, PBKDF2SubkeyLength);

        byte[] actualSubkey = GenerateSaltedHash(providedPassword, salt, PBKDF2IterCount, PBKDF2SubkeyLength);

        return ByteArraysEqual(actualSubkey, expectedSubkey)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }

    private static byte[] GenerateSaltedHash(string password, byte[] salt, int iterCount, int outputBytes)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterCount, HashAlgorithmName.SHA1))
        {
            return pbkdf2.GetBytes(outputBytes);
        }
    }

    private static bool ByteArraysEqual(byte[] a, byte[] b)
    {
        if (a == null && b == null)
            return true;
        if (a == null || b == null || a.Length != b.Length)
            return false;

        var areSame = true;
        for (var i = 0; i < a.Length; i++)
        {
            areSame &= (a[i] == b[i]);
        }

        return areSame;
    }
}

//public class LegacyPasswordHasher : Microsoft.AspNetCore.Identity.IPasswordHasher<ApplicationUser>
//{
//    private readonly Microsoft.AspNet.Identity.PasswordHasher _passwordHasher = new Microsoft.AspNet.Identity.PasswordHasher();

//    public string HashPassword(ApplicationUser user, string password)
//    {
//        // Uses ASP.NET Identity 2.x default implementation
//        return _passwordHasher.HashPassword(password);
//    }

//    public Microsoft.AspNetCore.Identity.PasswordVerificationResult VerifyHashedPassword(
//        ApplicationUser user, string hashedPassword, string providedPassword)
//    {
//        // Adjusted to match the required return type
//        var result = _passwordHasher.VerifyHashedPassword(hashedPassword, providedPassword);
//        switch (result)
//        {
//            case Microsoft.AspNet.Identity.PasswordVerificationResult.Success:
//                return Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success;
//            case Microsoft.AspNet.Identity.PasswordVerificationResult.SuccessRehashNeeded:
//                return Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success;
//            //case Microsoft.AspNet.Identity.PasswordVerificationResult.SuccessRehashNeeded:
//            //    return Microsoft.AspNetCore.Identity.PasswordVerificationResult.SuccessRehashNeeded;
//            case Microsoft.AspNet.Identity.PasswordVerificationResult.Failed:
//            default:
//                return Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed;
//        }

//    }


//}
