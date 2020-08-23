using System;
using System.Collections.Generic;
using System.Text;

namespace HttPlaceholder.Client.Client
{
    public static class ConfigurationExtensions
    {
        public static Configuration AddBasicAuthentication(
            this Configuration configuration,
            string username,
            string password)
        {
            if (configuration.DefaultHeaders == null)
            {
                configuration.DefaultHeaders = new Dictionary<string, string>();
            }

            configuration.DefaultHeaders.Add("Authorization",
                $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"))}");
            return configuration;
        }
    }
}