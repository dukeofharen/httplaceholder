using System.Threading.Tasks;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new Configuration
            {
                BasePath = "http://localhost:5000"
            }.AddBasicAuthentication("duco", "pass"); // This is optional: only do this if the API is secured.
            var requests = new RequestApi(config); 
            var requestResult = await requests.RequestGetAllAsync();
        }
    }
}