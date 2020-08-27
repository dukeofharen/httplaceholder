using System;
using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations
{
    [TestClass]
    public class RequestLoggerFacts
    {
        private readonly DateTime _utcNow = DateTime.UtcNow;
        private readonly Mock<IDateTime> _dateTimeMock = new Mock<IDateTime>();
        private RequestLogger _logger;

        [TestInitialize]
        public void Initialize()
        {
            _dateTimeMock
                .Setup(m => m.UtcNow)
                .Returns(_utcNow);
            _logger = new RequestLogger(_dateTimeMock.Object);
        }

        [TestMethod]
        public void RequestLogger_GetResult_HappyFlow()
        {
            // act
            var result = _logger.GetResult();

            // assert
            Assert.AreEqual(_utcNow, result.RequestBeginTime);
            Assert.AreEqual(_utcNow, result.RequestEndTime);
        }

        [TestMethod]
        public void RequestLogger_LogRequestParameters_HappyFlow()
        {
            // arrange
            const string method = "POST";
            const string url = "https://google.com";
            const string body = "HACKING GOOGLE!";
            const string clientIp = "127.0.0.1";
            var headers = new Dictionary<string, string>();

            // act
            _logger.LogRequestParameters(method, url, body, clientIp, headers);
            var result = _logger.GetResult();

            // assert
            Assert.AreEqual(method, result.RequestParameters.Method);
            Assert.AreEqual(url, result.RequestParameters.Url);
            Assert.AreEqual(body, result.RequestParameters.Body);
            Assert.AreEqual(clientIp, result.RequestParameters.ClientIp);
            Assert.AreEqual(headers, result.RequestParameters.Headers);
        }

        [TestMethod]
        public void RequestLogger_SetCorrelationId_HappyFlow()
        {
            // arrange
            var correlationId = Guid.NewGuid().ToString();

            // act
            _logger.SetCorrelationId(correlationId);
            var result = _logger.GetResult();

            // assert
            Assert.AreEqual(correlationId, result.CorrelationId);
        }

        [TestMethod]
        public void RequestLogger_SetStubExecutionResult_HappyFlow()
        {
            // arrange
            const string stubId = "stub-01";

            var condition1 = new ConditionCheckResultModel
            {
                CheckerName = Guid.NewGuid().ToString(),
                ConditionValidation = ConditionValidationType.Invalid
            };
            var condition2 = new ConditionCheckResultModel
            {
                CheckerName = Guid.NewGuid().ToString(),
                ConditionValidation = ConditionValidationType.Valid
            };
            var condition3 = new ConditionCheckResultModel
            {
                CheckerName = Guid.NewGuid().ToString(),
                ConditionValidation = ConditionValidationType.NotExecuted
            };
            var conditions = new[]
            {
            condition1,
            condition2,
            condition3
         };

            var negativeCondition1 = new ConditionCheckResultModel
            {
                CheckerName = Guid.NewGuid().ToString(),
                ConditionValidation = ConditionValidationType.Invalid
            };
            var negativeCondition2 = new ConditionCheckResultModel
            {
                CheckerName = Guid.NewGuid().ToString(),
                ConditionValidation = ConditionValidationType.NotExecuted
            };
            var negativeCondition3 = new ConditionCheckResultModel
            {
                CheckerName = Guid.NewGuid().ToString(),
                ConditionValidation = ConditionValidationType.Valid
            };

            // act
            _logger.SetStubExecutionResult(stubId, false, conditions);
            var result = _logger.GetResult();

            // assert
            var executionResult = result.StubExecutionResults.Single();
            Assert.AreEqual(stubId, executionResult.StubId);
            Assert.IsFalse(executionResult.Passed);

            var conditionsResult = executionResult.Conditions.ToArray();
            Assert.AreEqual(2, conditionsResult.Length);
            Assert.AreEqual(condition1, conditionsResult[0]);
            Assert.AreEqual(condition2, conditionsResult[1]);
        }

        [TestMethod]
        public void RequestLogger_SetExecutingStubId_HappyFlow()
        {
            // arrange
            const string stubId = "stub-01";

            // act
            _logger.SetExecutingStubId(stubId);
            var result = _logger.GetResult();

            // assert
            Assert.AreEqual(stubId, result.ExecutingStubId);
        }

        [TestMethod]
        public void RequestLogger_SetResponseWriterResult_HappyFlow()
        {
            // arrange
            var resultModel = new StubResponseWriterResultModel
            {
                Executed = true, Log = string.Empty, ResponseWriterName = "Writer"
            };

            // act
            _logger.SetResponseWriterResult(resultModel);
            var result = _logger.GetResult();

            // assert
            var responseWriterResult = result.StubResponseWriterResults.Single();
            Assert.AreEqual(resultModel, responseWriterResult);
        }
    }
}
