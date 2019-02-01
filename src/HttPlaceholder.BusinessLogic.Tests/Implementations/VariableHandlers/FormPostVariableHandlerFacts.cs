using System.Collections.Generic;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.BusinessLogic.Implementations.VariableHandlers;
using HttPlaceholder.Models;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.VariableHandlers
{
    [TestClass]
    public class FormPostVariableHandlerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private FormPostVariableHandler _handler;

        [TestInitialize]
        public void Initialize()
        {
            _handler = new FormPostVariableHandler(_httpContextServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpContextServiceMock.VerifyAll();
        }

        [TestMethod]
        public void FormPostVariableHandler_Parse_HappyFlow()
        {
            // arrange
            string input = "Form var 1: ((form_post:var1)), Form var 2: ((form_post:var2)), Form var 3: ((form_post:var3))";

            var formTuples = new[]
            {
                ("var1", new StringValues("https://google.com")),
                ("var3", new StringValues("value3"))
            };

            string expectedResult = "Form var 1: https://google.com, Form var 2: , Form var 3: value3";

            _httpContextServiceMock
                .Setup(m => m.GetFormValues())
                .Returns(formTuples);

            // act
            var matches = Constants.Regexes.VarRegex.Matches(input);
            string result = _handler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
