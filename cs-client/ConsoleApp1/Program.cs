using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var requests = new RequestApi(new Configuration
                {
                    BasePath = "http://localhost:5000",
                    DefaultHeaders = new Dictionary<string, string>
                    {
                        {"Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("duco:pass"))}"}
                    }
                });
                var requestResult = await requests.RequestGetAllAsync();
                var requestResult1 = await requests.RequestGetByStubIdAsync("test123");
            }
            catch (Exception ex)
            {
                ;
            }
        }
    }
}