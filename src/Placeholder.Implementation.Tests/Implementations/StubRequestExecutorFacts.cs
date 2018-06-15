using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            Id = Guid.NewGuid().ToString(),
            Conditions = new StubConditionsModel()
         };
         _stub2 = new StubModel
         {
            Id = Guid.NewGuid().ToString(),
            Conditions = new StubConditionsModel()
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
      public async Task StubRequestExecutor_ExecuteRequestAsync_NoConditionPassed_ShouldThrowException()
      {
         // arrange
         _conditionCheckerMock1
            .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<StubConditionsModel>()))
            .Returns(ConditionValidationType.Invalid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<StubConditionsModel>()))
            .Returns(ConditionValidationType.Invalid);

         // act
         var exception = await Assert.ThrowsExceptionAsync<RequestValidationException>(() => _executor.ExecuteRequestAsync());

         // assert
         Assert.IsTrue(exception.Message.Contains("and the request did not pass"));
      }

      [TestMethod]
      public async Task StubRequestExecutor_ExecuteRequestAsync_NoConditionExecuted_ShouldThrowException()
      {
         // arrange
         _conditionCheckerMock1
            .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<StubConditionsModel>()))
            .Returns(ConditionValidationType.NotExecuted);
         _conditionCheckerMock2
            .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<StubConditionsModel>()))
            .Returns(ConditionValidationType.NotExecuted);

         // act
         var exception = await Assert.ThrowsExceptionAsync<RequestValidationException>(() => _executor.ExecuteRequestAsync());

         // assert
         Assert.IsTrue(exception.Message.Contains("and the request did not pass"));
      }

      [TestMethod]
      public async Task StubRequestExecutor_ExecuteRequestAsync_MultipleValidStubs_ShouldPickFirstOne()
      {
         // arrange
         var expectedResponseModel = new ResponseModel();
         _conditionCheckerMock1
            .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<StubConditionsModel>()))
            .Returns(ConditionValidationType.Valid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<StubConditionsModel>()))
            .Returns(ConditionValidationType.Valid);

         _stubResponseGeneratorMock
            .Setup(m => m.GenerateResponseAsync(_stub1))
            .ReturnsAsync(expectedResponseModel);

         // act
         var response = await _executor.ExecuteRequestAsync();

         // assert
         Assert.AreEqual(expectedResponseModel, response);
      }

      [TestMethod]
      public async Task StubRequestExecutor_ExecuteRequestAsync_HappyFlow()
      {
         // arrange
         var expectedResponseModel = new ResponseModel();
         _conditionCheckerMock1
            .Setup(m => m.Validate(_stub1.Id, _stub1.Conditions))
            .Returns(ConditionValidationType.Invalid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(_stub1.Id, _stub1.Conditions))
            .Returns(ConditionValidationType.Invalid);
         _conditionCheckerMock1
            .Setup(m => m.Validate(_stub2.Id, _stub2.Conditions))
            .Returns(ConditionValidationType.Valid);
         _conditionCheckerMock2
            .Setup(m => m.Validate(_stub2.Id, _stub2.Conditions))
            .Returns(ConditionValidationType.Valid);
         _stubResponseGeneratorMock
            .Setup(m => m.GenerateResponseAsync(_stub2))
            .ReturnsAsync(expectedResponseModel);

         // act
         var response = await _executor.ExecuteRequestAsync();

         // assert
         Assert.AreEqual(expectedResponseModel, response);
      }
   }
}
