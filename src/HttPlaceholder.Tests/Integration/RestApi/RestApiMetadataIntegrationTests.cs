using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Dto.v1.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    [TestClass]
    public class RestApiMetadataIntegrationTests : RestApiIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize() => InitializeRestApiIntegrationTest();

        [TestCleanup]
        public void Cleanup() => CleanupRestApiIntegrationTest();

        [TestMethod]
        public async Task RestApiIntegration_Metadata_Get_CredentialsAreCorrect_UsernameMatches_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/ph-api/metadata")
            {
                Headers = {{"Accept", "application/json"}}
            };

            // Act
            using var response = await Client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var metadata = JsonConvert.DeserializeObject<MetadataDto>(content);

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(metadata.Version));
            Assert.AreEqual(10, metadata.VariableHandlers.Count());
        }
    }
}
