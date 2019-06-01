using System;
using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations
{
    [TestClass]
    public class RequestLoggerFacts
    {
        private RequestLogger _logger = new RequestLogger();

        [TestMethod]
        public void RequestLogger_GetResult_HappyFlow()
        {
            // act
            var result = _logger.GetResult();

            // assert
            Assert.AreEqual(DateTime.Now.Date, result.RequestBeginTime.Date);
            Assert.AreEqual(DateTime.Now.Date, result.RequestEndTime.Date);
        }

        [TestMethod]
        public void RequestLogger_LogRequestParameters_HappyFlow()
        {
            // arrange
            var method = "POST";
            var url = "https://google.com";
            var body = "HACKING GOOGLE!";
            var clientIp = "127.0.0.1";
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
            var stubId = "stub-01";

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
            var negativeConditions = new[]
            {
            negativeCondition1,
            negativeCondition2,
            negativeCondition3
         };

            // act
            _logger.SetStubExecutionResult(stubId, false, conditions, negativeConditions);
            var result = _logger.GetResult();

            // assert
            var executionResult = result.StubExecutionResults.Single();
            Assert.AreEqual(stubId, executionResult.StubId);
            Assert.IsFalse(executionResult.Passed);

            var conditionsResult = executionResult.Conditions.ToArray();
            Assert.AreEqual(2, conditionsResult.Length);
            Assert.AreEqual(condition1, conditionsResult[0]);
            Assert.AreEqual(condition2, conditionsResult[1]);

            var negativeConditionsResult = executionResult.NegativeConditions.ToArray();
            Assert.AreEqual(2, negativeConditionsResult.Length);
            Assert.AreEqual(negativeCondition1, negativeConditionsResult[0]);
            Assert.AreEqual(negativeCondition3, negativeConditionsResult[1]);
        }

        [TestMethod]
        public void RequestLogger_SetExecutingStubId_HappyFlow()
        {
            // arrange
            var stubId = "stub-01";

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
            var writerName = "HtmlWriter";
            var executed = true;

            // act
            _logger.SetResponseWriterResult(writerName, executed);
            var result = _logger.GetResult();

            // assert
            var responseWriterResult = result.StubResponseWriterResults.Single();
            Assert.AreEqual(writerName, responseWriterResult.ResponseWriterName);
            Assert.AreEqual(executed, responseWriterResult.Executed);
        }
    }
}
