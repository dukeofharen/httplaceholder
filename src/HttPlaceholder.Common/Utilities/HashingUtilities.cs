using System;
using System.Security.Cryptography;
using System.Text;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
/// A utility class that contains hashing methods.
/// </summary>
public static class HashingUtilities
{
    /// <summary>
    /// Calculates an MD5 string from a specific input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The hashed MD5 string.</returns>
    public static string GetMd5String(string input)
    {
        using var md5 = MD5.Create();
        var data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        var builder = new StringBuilder();
        foreach (var t in data)
        {
            builder.Append(t.ToString("x2"));
        }

        return builder.ToString();
    }

    /// <summary>
    /// Calculates an SHA1 string from a specific input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The hashed SHA1 string.</returns>
    public static string GetSha512String(string input)
    {
        using var sha = SHA512.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
