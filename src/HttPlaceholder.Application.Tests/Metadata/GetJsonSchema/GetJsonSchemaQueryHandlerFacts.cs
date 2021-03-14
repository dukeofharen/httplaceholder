using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Metadata.Queries.GetJsonSchema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.Tests.Metadata.GetJsonSchema
{
    [TestClass]
    public class GetJsonSchemaQueryHandlerFacts
    {
        private readonly GetJsonSchemaQueryHandler _handler = new GetJsonSchemaQueryHandler();

        [TestMethod]
        public async Task Handle_NoArray()
        {
            // Arrange
            var query = new GetJsonSchemaQuery(false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Contains("\"title\": \"StubModel\","));
            JToken.Parse(result);
        }

        [TestMethod]
        public async Task Handle_Array()
        {
            // Arrange
            var query = new GetJsonSchemaQuery(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Contains("\"title\": \"StubModel[]\","));
            JToken.Parse(result);
        }
    }
}
