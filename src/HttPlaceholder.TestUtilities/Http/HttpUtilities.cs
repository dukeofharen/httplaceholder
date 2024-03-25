using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.TestUtilities.Http;

public static class HttpUtilities
{
    public static string GetBasicAuthHeaderValue(string username, string password) =>
        $"Basic {$"{username}:{password}".Base64Encode()}";
}
