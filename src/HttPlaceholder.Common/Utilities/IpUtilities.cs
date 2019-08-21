namespace HttPlaceholder.Common.Utilities
{
    public static class IpUtilities
    {
        public static string NormalizeIp(string ip) => ip == "::1" ? "127.0.0.1" : ip;
    }
}
