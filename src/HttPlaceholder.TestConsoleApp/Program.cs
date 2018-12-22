using System.Threading.Tasks;
using HttPlaceholder.Client;
using HttPlaceholder.Client.Models;
using HttPlaceholder.Models;

namespace HttPlaceholder.TestConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var settings = new HttPlaceholderSettingsModel
            {
                RootUrl = "http://localhost:5000",
                Username = "user",
                Password = "pass"
            };
            var client = new HttPlaceholderClient(settings);
            //var result = await client.GetUserAsync("duco", "ducoducoduco");
            //var result = await client.GetMetadataAsync();
            //var result = await client.GetAllRequestsAsync();
            //var result = await client.GetAllRequestsByStubIdAsync("unique-stub-id");
            //await client.DeleteAllRequestsAsync();
            //var result = await client.GetStubAsync("main-page");
            //var result = await client.GetAllStubsAsync();
            //await client.AddStubAsync(new StubModel
            //{
            //    Id = "Duco",
            //    Conditions = new StubConditionsModel
            //    {
            //        Url = new StubUrlConditionModel
            //        {
            //            Path = "/ducopietje"
            //        }
            //    },
            //    Response = new StubResponseModel
            //    {
            //        Text = "NOU MOOI!!!"
            //    }
            //});
            //await client.DeleteStubAsync("DucoPietje");
            //var result = await client.GetAllStubsInTenant("simple-site");
            //await client.DeleteAllStubsInTenant("simple-site");
            await client.UpdateAllStubsInTenant("duuuuco", new[]
            {
                new StubModel
                {
                    Id = "Duco",
                    Conditions = new StubConditionsModel
                    {
                        Url = new StubUrlConditionModel
                        {
                            Path = "/ducopietje"
                        }
                    },
                    Response = new StubResponseModel
                    {
                        Text = "NOU MOOI!!!"
                    }
                }
            });
        }
    }
}
