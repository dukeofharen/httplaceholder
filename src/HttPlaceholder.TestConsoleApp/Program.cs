using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client;

namespace HttPlaceholder.TestConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string baseUrl = "http://localhost:5000";
            var httpClient = new HttpClient();
            //var client = new MetadataClient(httpClient)
            //{
            //    BaseUrl = "http://localhost:5000"
            //};
            //var result = await client.GetAsync();

            //var client = new RequestClient(httpClient)
            //{
            //    BaseUrl = baseUrl
            //};
            //var result = await client.GetAllAsync();

            //var client = new StubClient(httpClient)
            //{
            //    BaseUrl = baseUrl
            //};
            //var result = await client.GetAllAsync();

            var client = new TenantClient(httpClient)
            {
                BaseUrl = baseUrl
            };
            var result = await client.GetAllAsync("01-get");
        }
    }
}
