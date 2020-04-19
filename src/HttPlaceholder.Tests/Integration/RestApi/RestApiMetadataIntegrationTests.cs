using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            // Act
            var result = await GetFactory().MetadataClient.GetAsync();

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Version));
        }
    }
}
