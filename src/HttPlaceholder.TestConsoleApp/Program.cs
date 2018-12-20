using System;
using System.Threading.Tasks;
using HttPlaceholder.Client;
using HttPlaceholder.Client.Models;

namespace HttPlaceholder.TestConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var settings = new HttPlaceholderSettingsModel
            {
                RootUrl = "http://localhost:5000"
            };
            var client = new HttPlaceholderClient(settings);
            var result = await client.GetUserAsync("duco", "ducoducoduco");
        }
    }
}
