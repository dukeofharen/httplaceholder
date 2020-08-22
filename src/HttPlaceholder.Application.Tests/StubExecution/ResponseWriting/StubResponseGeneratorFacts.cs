using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriting;
using HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations;
using HttPlaceholder.Domain;
using HttPlaceholder.TestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriting
{
    [TestClass]
    public class StubResponseGeneratorFacts
    {
        private readonly IList<IResponseWriter> _responseWriters = new List<IResponseWriter>();
        private StubResponseGenerator _generator;

        [TestInitialize]
        public void Initialize() =>
            _generator = new StubResponseGenerator(
                TestObjectFactory.GetRequestLoggerFactory(),
                _responseWriters);

        [TestMethod]
        public async Task StubResponseGenerator_GenerateResponseAsync_HappyFlow()
        {
            // arrange
            var stub = new StubModel();
            var responseWriterMock1 = new Mock<IResponseWriter>();
            var responseWriterMock2 = new Mock<IResponseWriter>();

            responseWriterMock1
               .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>()))
               .Callback<StubModel, ResponseModel>((s, r) => r.StatusCode = 401)
               .ReturnsAsync(StubResponseWriterResultModel.IsExecuted(GetType().Name));

            responseWriterMock2
               .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>()))
               .Callback<StubModel, ResponseModel>((s, r) => r.Headers.Add("X-Api-Key", "12345"))
               .ReturnsAsync(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));

            _responseWriters.Add(responseWriterMock1.Object);
            _responseWriters.Add(responseWriterMock2.Object);

            // act
            var result = await _generator.GenerateResponseAsync(stub);

            // assert
            Assert.AreEqual(401, result.StatusCode);
            Assert.AreEqual("12345", result.Headers["X-Api-Key"]);
        }

        [TestMethod]
        public async Task StubResponseGenerator_GenerateResponseAsync_HappyFlow_ResponseWriterPriority()
        {
            // arrange
            var stub = new StubModel();
            var responseWriterMock1 = new Mock<IResponseWriter>();
            var responseWriterMock2 = new Mock<IResponseWriter>();

            responseWriterMock1
               .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>()))
               .Callback<StubModel, ResponseModel>((s, r) => r.StatusCode = 401)
               .ReturnsAsync(StubResponseWriterResultModel.IsExecuted(GetType().Name));
            responseWriterMock1
                .Setup(m => m.Priority)
                .Returns(10);

            responseWriterMock2
               .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>()))
               .Callback<StubModel, ResponseModel>((s, r) => r.StatusCode = 404)
               .ReturnsAsync(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
            responseWriterMock2
                .Setup(m => m.Priority)
                .Returns(-10);

            _responseWriters.Add(responseWriterMock1.Object);
            _responseWriters.Add(responseWriterMock2.Object);

            // act
            var result = await _generator.GenerateResponseAsync(stub);

            // assert
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}
