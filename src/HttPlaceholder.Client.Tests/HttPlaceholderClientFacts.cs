using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using HttPlaceholder.Client.Models;
using HttPlaceholder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Client.Tests
{
    [TestClass]
    public class HttPlaceholderClientFacts
    {
        private const string RootUrl = "http://localhost:5000";
        private const string Username = "user";
        private const string Password = "pass";
        private HttPlaceholderClient _client;

        [TestInitialize]
        public void Initialize()
        {
            _client = new HttPlaceholderClient(new HttPlaceholderSettingsModel
            {
                RootUrl = RootUrl,
                Username = Username,
                Password = Password
            });
        }

        [TestMethod]
        public async Task HttPlaceholderClient_GetUserAsync_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // arrange
                httpTest.RespondWithJson(new
                {
                    username = Username
                });

                // act
                var result = await _client.GetUserAsync(Username, Password);

                // assert
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/users/{Username}")
                    .WithVerb(HttpMethod.Get)
                    .WithBasicAuth(Username, Password);
                Assert.AreEqual(Username, result.Username);
            }
        }

        [TestMethod]
        public async Task HttPlaceholderClient_GetMetadataAsync_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // arrange
                string version = "1.2.3.4";
                httpTest.RespondWithJson(new
                {
                    Version = version
                });

                // act
                var result = await _client.GetMetadataAsync();

                // assert
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/metadata")
                    .WithVerb(HttpMethod.Get);
                Assert.AreEqual(version, result.Version);
            }
        }

        [TestMethod]
        public async Task HttPlaceholderClient_GetAllRequestsAsync_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // arrange
                httpTest.RespondWith(TestResources.get_all_requests_example);

                // act
                var result = (await _client.GetAllRequestsAsync()).ToArray();

                // assert
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/requests")
                    .WithVerb(HttpMethod.Get)
                    .WithBasicAuth(Username, Password);
                Assert.AreEqual(2, result.Length);
                Assert.AreEqual("ae4ed085-ba9c-4d17-9b4c-57ae50533d8d", result[0].CorrelationId);
                Assert.AreEqual("7fd13aa3-6c43-4dee-8ca7-d3ae9b22bcf0", result[1].CorrelationId);
            }
        }

        [TestMethod]
        public async Task HttPlaceholderClient_GetAllRequestsByStubIdAsync_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // arrange
                string stubId = "bla";
                httpTest.RespondWith(TestResources.get_all_requests_example);

                // act
                var result = (await _client.GetAllRequestsByStubIdAsync(stubId)).ToArray();

                // assert
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/requests/{stubId}")
                    .WithVerb(HttpMethod.Get)
                    .WithBasicAuth(Username, Password);
                Assert.AreEqual(2, result.Length);
                Assert.AreEqual("ae4ed085-ba9c-4d17-9b4c-57ae50533d8d", result[0].CorrelationId);
                Assert.AreEqual("7fd13aa3-6c43-4dee-8ca7-d3ae9b22bcf0", result[1].CorrelationId);
            }
        }

        [TestMethod]
        public async Task HttPlaceholderClient_DeleteAllRequestsAsync_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // act
                await _client.DeleteAllRequestsAsync();

                // assert
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/requests")
                    .WithVerb(HttpMethod.Delete)
                    .WithBasicAuth(Username, Password);
            }
        }

        [TestMethod]
        public async Task HttPlaceholderClient_GetStubAsync_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // arrange
                string stubId = "main-page";
                httpTest.RespondWith(TestResources.get_stub_by_id_example);

                // act
                var result = await _client.GetStubAsync(stubId);

                // assert
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/stubs/{stubId}")
                    .WithVerb(HttpMethod.Get)
                    .WithBasicAuth(Username, Password);
                Assert.AreEqual(stubId, result.Stub.Id);
            }
        }

        [TestMethod]
        public async Task HttPlaceholderClient_GetAllStubsAsync_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // arrange
                httpTest.RespondWith(TestResources.get_all_stubs_example);

                // act
                var result = (await _client.GetAllStubsAsync()).ToArray();

                // assert
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/stubs")
                    .WithVerb(HttpMethod.Get)
                    .WithBasicAuth(Username, Password);
                Assert.AreEqual(4, result.Length);
                Assert.AreEqual("image", result[0].Stub.Id);
                Assert.AreEqual("main-page", result[1].Stub.Id);
                Assert.AreEqual("style-css", result[2].Stub.Id);
                Assert.AreEqual("unique-stub-id", result[3].Stub.Id);
            }
        }

        [TestMethod]
        public async Task HttPlaceholderClient_AddStubAsync_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // arrange
                var newStub = new StubModel
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
                };

                // act
                await _client.AddStubAsync(newStub);

                // arrange
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/stubs")
                    .WithVerb(HttpMethod.Post)
                    .WithBasicAuth(Username, Password)
                    .WithRequestJson(newStub);
            }
        }

        [TestMethod]
        public async Task HttPlaceholderClient_DeleteStubAsync_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // arrange
                string stubId = "bla";

                // act
                await _client.DeleteStubAsync(stubId);

                // assert
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/stubs/{stubId}")
                    .WithVerb(HttpMethod.Delete)
                    .WithBasicAuth(Username, Password);
            }
        }

        [TestMethod]
        public async Task HttPlaceholderClient_GetAllStubsInTenant_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // arrange
                string tenant = "simple-site";
                httpTest.RespondWith(TestResources.get_stubs_by_tenant_example);

                // act
                var result = (await _client.GetAllStubsInTenant(tenant)).ToArray();

                // assert
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/tenants/{tenant}/stubs")
                    .WithVerb(HttpMethod.Get)
                    .WithBasicAuth(Username, Password);
                Assert.AreEqual(3, result.Length);
                Assert.AreEqual("image", result[0].Stub.Id);
                Assert.AreEqual("main-page", result[1].Stub.Id);
                Assert.AreEqual("style-css", result[2].Stub.Id);
            }
        }

        [TestMethod]
        public async Task HttPlaceholderClient_DeleteAllStubsInTenant_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // arrange
                string tenant = "simple-site";

                // act
                await _client.DeleteAllStubsInTenant(tenant);

                // assert
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/tenants/{tenant}/stubs")
                    .WithVerb(HttpMethod.Delete)
                    .WithBasicAuth(Username, Password);
            }
        }

        [TestMethod]
        public async Task HttPlaceholderClient_UpdateAllStubsInTenant_HappyFlow()
        {
            using (var httpTest = new HttpTest())
            {
                // arrange
                string tenant = "simple-site";
                var stubs = new[]
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
                };

                // act
                await _client.UpdateAllStubsInTenant(tenant, stubs);

                // assert
                httpTest
                    .ShouldHaveCalled($"{RootUrl}/ph-api/tenants/{tenant}/stubs")
                    .WithVerb(HttpMethod.Put)
                    .WithBasicAuth(Username, Password)
                    .WithRequestJson(stubs);
            }
        }
    }
}
