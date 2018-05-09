using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Placeholder.Exceptions;
using Placeholder.Implementation.Implementations;
using Placeholder.Implementation.Tests.Utilities;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Tests.Implementations
{
   [TestClass]
   public class StubRequestExecutorFacts
   {
      private Mock<IServiceProvider> _serviceProviderMock;
      private Mock<IStubManager> _stubManagerMock;
      private Mock<IStubResponseGenerator> _stubResponseGeneratorMock;
      private Mock<IConditionChecker> _conditionCheckerMock1;
      private Mock<IConditionChecker> _conditionCheckerMock2;
      private StubModel _stub1;
      private StubModel _stub2;
      private StubRequestExecutor _executor;

      [TestInitialize]
      public void Initialize()
      {
         _serviceProviderMock = new Mock<IServiceProvider>();
         _stubManagerMock = new Mock<IStubManager>();
         _stubResponseGeneratorMock = new Mock<IStubResponseGenerator>();
         _executor = new StubRequestExecutor(
            TestObjectFactory.GetRequestLoggerFactory(),
            _serviceProviderMock.Object,
            _stubManagerMock.Object,
            _stubResponseGeneratorMock.Object);

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
         _serviceProviderMock.VerifyAll();
         _stubManagerMock.VerifyAll();
         _stubResponseGeneratorMock.VerifyAll();
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
      public void StubRequestExecutor_ExecuteRequest_MultipleValidStubs_ShouldPickFirstOne()
      {
         // arrange
         var expectedResponseModel = new ResponseModel();
         _conditionCheckerMock1
            .Setup(m => m.Validate(It.IsAny<StubModel>()))
            .Returns(ConditionValidationType.Valid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(It.IsAny<StubModel>()))
            .Returns(ConditionValidationType.Valid);

         _stubManagerMock
            .Setup(m => m.GetStubById(_stub1.Id))
            .Returns(_stub1);

         _stubResponseGeneratorMock
            .Setup(m => m.GenerateResponse(_stub1))
            .Returns(expectedResponseModel);

         // act
         var response = _executor.ExecuteRequest();

         // assert
         Assert.AreEqual(expectedResponseModel, response);
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
         var expectedResponseModel = new ResponseModel();
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
         _stubResponseGeneratorMock
            .Setup(m => m.GenerateResponse(_stub2))
            .Returns(expectedResponseModel);

         // act
         var response = _executor.ExecuteRequest();

         // assert
         Assert.AreEqual(expectedResponseModel, response);
      }
   }
}
