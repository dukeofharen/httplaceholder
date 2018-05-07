using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Placeholder.Exceptions;
using Placeholder.Implementation.Implementations;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Tests.Implementations
{
   [TestClass]
   public class StubRequestExecutorFacts
   {
      private Mock<ILogger<StubRequestExecutor>> _loggerMock;
      private Mock<IServiceProvider> _serviceProviderMock;
      private Mock<IStubManager> _stubManagerMock;
      private Mock<IConditionChecker> _conditionCheckerMock1;
      private Mock<IConditionChecker> _conditionCheckerMock2;
      private StubModel _stub1;
      private StubModel _stub2;
      private StubRequestExecutor _executor;

      [TestInitialize]
      public void Initialize()
      {
         _loggerMock = new Mock<ILogger<StubRequestExecutor>>();
         _serviceProviderMock = new Mock<IServiceProvider>();
         _stubManagerMock = new Mock<IStubManager>();
         _executor = new StubRequestExecutor(
            _loggerMock.Object,
            _serviceProviderMock.Object,
            _stubManagerMock.Object);

         _conditionCheckerMock1 = new Mock<IConditionChecker>();
         _conditionCheckerMock2 = new Mock<IConditionChecker>();
         _serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IConditionChecker>)))
            .Returns(new[] { _conditionCheckerMock1.Object, _conditionCheckerMock2.Object });

         _stub1 = new StubModel
         {
            Id = Guid.NewGuid().ToString()
         };
         _stub2 = new StubModel
         {
            Id = Guid.NewGuid().ToString()
         };
         _stubManagerMock
            .Setup(m => m.Stubs)
            .Returns(new[] { _stub1, _stub2 });
      }

      [TestCleanup]
      public void Cleanup()
      {
         _loggerMock.VerifyAll();
         _serviceProviderMock.VerifyAll();
         _stubManagerMock.VerifyAll();
      }

      [TestMethod]
      public void StubRequestExecutor_ExecuteRequest_NoConditionPassed_ShouldThrowException()
      {
         // arrange
         _conditionCheckerMock1
            .Setup(m => m.Validate(It.IsAny<StubModel>()))
            .Returns(ConditionValidationType.Invalid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(It.IsAny<StubModel>()))
            .Returns(ConditionValidationType.Invalid);

         // act
         var exception = Assert.ThrowsException<RequestValidationException>(() => _executor.ExecuteRequest());

         // assert
         Assert.IsTrue(exception.Message.Contains("and the request did not pass"));
      }

      [TestMethod]
      public void StubRequestExecutor_ExecuteRequest_NoConditionExecuted_ShouldThrowException()
      {
         // arrange
         _conditionCheckerMock1
            .Setup(m => m.Validate(It.IsAny<StubModel>()))
            .Returns(ConditionValidationType.NotExecuted);
         _conditionCheckerMock2
            .Setup(m => m.Validate(It.IsAny<StubModel>()))
            .Returns(ConditionValidationType.NotExecuted);

         // act
         var exception = Assert.ThrowsException<RequestValidationException>(() => _executor.ExecuteRequest());

         // assert
         Assert.IsTrue(exception.Message.Contains("and the request did not pass"));
      }

      [TestMethod]
      public void StubRequestExecutor_ExecuteRequest_MultipleValidStubs_ShouldThrowException()
      {
         // arrange
         _conditionCheckerMock1
            .Setup(m => m.Validate(It.IsAny<StubModel>()))
            .Returns(ConditionValidationType.Valid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(It.IsAny<StubModel>()))
            .Returns(ConditionValidationType.Valid);

         // act
         var exception = Assert.ThrowsException<RequestValidationException>(() => _executor.ExecuteRequest());

         // assert
         Assert.IsTrue(exception.Message.Contains("which means no choice can be made"));
      }

      [TestMethod]
      public void StubRequestExecutor_ExecuteRequest_StubUnexpectedlyNotFound_ShouldThrowException()
      {
         // arrange
         _conditionCheckerMock1
            .Setup(m => m.Validate(_stub1))
            .Returns(ConditionValidationType.Invalid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(_stub1))
            .Returns(ConditionValidationType.Invalid);
         _conditionCheckerMock1
            .Setup(m => m.Validate(_stub2))
            .Returns(ConditionValidationType.Valid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(_stub2))
            .Returns(ConditionValidationType.Valid);
         _stubManagerMock
            .Setup(m => m.GetStubById(_stub2.Id))
            .Returns((StubModel)null);

         // act
         var exception = Assert.ThrowsException<RequestValidationException>(() => _executor.ExecuteRequest());

         // assert
         Assert.IsTrue(exception.Message.Contains("unexpectedly not found"));
      }

      [TestMethod]
      public void StubRequestExecutor_ExecuteRequest_HappyFlow()
      {
         // arrange
         _conditionCheckerMock1
            .Setup(m => m.Validate(_stub1))
            .Returns(ConditionValidationType.Invalid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(_stub1))
            .Returns(ConditionValidationType.Invalid);
         _conditionCheckerMock1
            .Setup(m => m.Validate(_stub2))
            .Returns(ConditionValidationType.Valid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(_stub2))
            .Returns(ConditionValidationType.Valid);
         _stubManagerMock
            .Setup(m => m.GetStubById(_stub2.Id))
            .Returns(_stub2);

         _stub2.Response = new StubResponseModel
         {
            Headers = new Dictionary<string, string>
            {
               {"X-Header", "value"}
            },
            StatusCode = 201,
            Text = "response"
         };

         // act
         var response = _executor.ExecuteRequest();

         // assert
         Assert.IsNotNull(response);
         Assert.AreEqual(_stub2.Response.StatusCode, response.StatusCode);
         Assert.AreEqual("value", response.Headers["X-Header"]);
         Assert.AreEqual(_stub2.Response.Text, Encoding.UTF8.GetString(response.Body));
      }

      [TestMethod]
      public void StubRequestExecutor_ExecuteRequest_HappyFlow_Base64Content()
      {
         // arrange
         _conditionCheckerMock1
            .Setup(m => m.Validate(_stub1))
            .Returns(ConditionValidationType.Invalid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(_stub1))
            .Returns(ConditionValidationType.Invalid);
         _conditionCheckerMock1
            .Setup(m => m.Validate(_stub2))
            .Returns(ConditionValidationType.Valid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(_stub2))
            .Returns(ConditionValidationType.Valid);
         _stubManagerMock
            .Setup(m => m.GetStubById(_stub2.Id))
            .Returns(_stub2);

         _stub2.Response = new StubResponseModel
         {
            Headers = new Dictionary<string, string>
            {
               {"X-Header", "value"}
            },
            StatusCode = 201,
            Base64 = "VGhpcyBpcyB0aGUgY29udGVudCE="
         };

         // act
         var response = _executor.ExecuteRequest();

         // assert
         Assert.IsNotNull(response);
         Assert.AreEqual(_stub2.Response.StatusCode, response.StatusCode);
         Assert.AreEqual("value", response.Headers["X-Header"]);
         Assert.AreEqual("This is the content!", Encoding.UTF8.GetString(response.Body));
      }
   }
}
