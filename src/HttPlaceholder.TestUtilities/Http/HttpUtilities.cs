using System;
using System.Text;

namespace HttPlaceholder.TestUtilities.Http
{
    public static class HttpUtilities
    {
        public static string GetBasicAuthHeaderValue(string username, string password) =>
            $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"))}";
    }
}
