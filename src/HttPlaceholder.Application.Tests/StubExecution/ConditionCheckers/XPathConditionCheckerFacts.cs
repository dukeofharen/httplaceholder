using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class XPathConditionCheckerFacts
{
    private readonly Mock<IHttpContextService> _httpContextServiceMock = new();
    private XPathConditionChecker _checker;

    [TestInitialize]
    public void Initialize() =>
        _checker = new XPathConditionChecker(
            _httpContextServiceMock.Object);

    [TestCleanup]
    public void Cleanup() => _httpContextServiceMock.VerifyAll();

    [TestMethod]
    public void XPathConditionChecker_Validate_StubsFound_ButNoXPathConditions_ShouldReturnNotExecuted()
    {
        // arrange
        var conditions = new StubConditionsModel
        {
            Xpath = null
        };

        // act
        var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public void XPathConditionChecker_Validate_StubsFound_XmlIsCorrupt_ShouldReturnInvalid()
    {
        // arrange
        const string body = @"<?xml version=""1.0""?>
<soap:Envelope>
  <soap:Header>
  </soap:Header>
  <soap:Body>
    <m:GetStockPrice>
      <m:StockName>Umbrella</m:StockName>
      <m:Description>An umbrella</m:Description>
    </m:GetStockPrice>
  </soap:Body>
</soap:Envelope>";
        var conditions = new StubConditionsModel
        {
            Xpath = new[]
            {
                new StubXpathModel
                {
                    QueryString = "/soap:Envelope/soap:Body/m:GetStockPrice/m:StockName[text() = 'Shades']",
                    Namespaces = new Dictionary<string, string>
                    {
                        { "soap", "http://www.w3.org/2003/05/soap-envelope" },
                        { "m", "http://www.example.org/stock/Reddy" }
                    }
                }
            }
        };

        _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

        // act
        var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        Assert.AreEqual("'soap' is an undeclared prefix. Line 2, position 2.", result.Log);
    }

    [TestMethod]
    public void XPathConditionChecker_Validate_StubsFound_AllXPathConditionsIncorrect_ShouldReturnInvalid()
    {
        // arrange
        const string body = @"<?xml version=""1.0""?>
<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:m=""http://www.example.org/stock/Reddy"">
  <soap:Header>
  </soap:Header>
  <soap:Body>
    <m:GetStockPrice>
      <m:StockName>Umbrella</m:StockName>
      <m:Description>An umbrella</m:Description>
    </m:GetStockPrice>
  </soap:Body>
</soap:Envelope>";
        var conditions = new StubConditionsModel
        {
            Xpath = new[]
            {
                new StubXpathModel
                {
                    QueryString = "/soap:Envelope/soap:Body/m:GetStockPrice/m:StockName[text() = 'Shades']",
                    Namespaces = new Dictionary<string, string>
                    {
                        { "soap", "http://www.w3.org/2003/05/soap-envelope" },
                        { "m", "http://www.example.org/stock/Reddy" }
                    }
                },
                new StubXpathModel
                {
                    QueryString = "/soap:Envelope/soap:Body/m:GetStockPrice/m:Description[text() = 'A pair of shades']",
                    Namespaces = new Dictionary<string, string>
                    {
                        { "soap", "http://www.w3.org/2003/05/soap-envelope" },
                        { "m", "http://www.example.org/stock/Reddy" }
                    }
                }
            }
        };

        _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

        // act
        var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void XPathConditionChecker_Validate_StubsFound_OnlyOneXPathConditionCorrect_ShouldReturnInvalid()
    {
        // arrange
        const string body = @"<?xml version=""1.0""?>
<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:m=""http://www.example.org/stock/Reddy"">
  <soap:Header>
  </soap:Header>
  <soap:Body>
    <m:GetStockPrice>
      <m:StockName>Umbrella</m:StockName>
      <m:Description>An umbrella</m:Description>
    </m:GetStockPrice>
  </soap:Body>
</soap:Envelope>";
        var conditions = new StubConditionsModel
        {
            Xpath = new[]
            {
                new StubXpathModel
                {
                    QueryString = "/soap:Envelope/soap:Body/m:GetStockPrice/m:StockName[text() = 'Umbrella']",
                    Namespaces = new Dictionary<string, string>
                    {
                        { "soap", "http://www.w3.org/2003/05/soap-envelope" },
                        { "m", "http://www.example.org/stock/Reddy" }
                    }
                },
                new StubXpathModel
                {
                    QueryString = "/soap:Envelope/soap:Body/m:GetStockPrice/m:Description[text() = 'A pair of shades']",
                    Namespaces = new Dictionary<string, string>
                    {
                        { "soap", "http://www.w3.org/2003/05/soap-envelope" },
                        { "m", "http://www.example.org/stock/Reddy" }
                    }
                }
            }
        };

        _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

        // act
        var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void XPathConditionChecker_Validate_StubsFound_HappyFlow_WithNamespaces()
    {
        // arrange
        const string body = @"<?xml version=""1.0""?>
<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:m=""http://www.example.org/stock/Reddy"">
  <soap:Header>
  </soap:Header>
  <soap:Body>
    <m:GetStockPrice>
      <m:StockName>Umbrella</m:StockName>
      <m:Description>An umbrella</m:Description>
    </m:GetStockPrice>
  </soap:Body>
</soap:Envelope>";
        var conditions = new StubConditionsModel
        {
            Xpath = new[]
            {
                new StubXpathModel
                {
                    QueryString = "/soap:Envelope/soap:Body/m:GetStockPrice/m:StockName[text() = 'Umbrella']",
                    Namespaces = new Dictionary<string, string>
                    {
                        { "soap", "http://www.w3.org/2003/05/soap-envelope" },
                        { "m", "http://www.example.org/stock/Reddy" }
                    }
                },
                new StubXpathModel
                {
                    QueryString = "/soap:Envelope/soap:Body/m:GetStockPrice/m:Description[text() = 'An umbrella']",
                    Namespaces = new Dictionary<string, string>
                    {
                        { "soap", "http://www.w3.org/2003/05/soap-envelope" },
                        { "m", "http://www.example.org/stock/Reddy" }
                    }
                }
            }
        };

        _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

        // act
        var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    [TestMethod]
    public void XPathConditionChecker_Validate_StubsFound_HappyFlow_WithNamespaces_FilteredByRegex()
    {
        // arrange
        const string body = @"<?xml version=""1.0""?>
<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:m=""http://www.example.org/stock/Reddy"">
  <soap:Header>
  </soap:Header>
  <soap:Body>
    <m:GetStockPrice>
      <m:StockName>Umbrella</m:StockName>
      <m:Description>An umbrella</m:Description>
    </m:GetStockPrice>
  </soap:Body>
</soap:Envelope>";
        var conditions = new StubConditionsModel
        {
            Xpath = new[]
            {
                new StubXpathModel
                {
                    QueryString = "/soap:Envelope/soap:Body/m:GetStockPrice/m:StockName[text() = 'Umbrella']"
                },
                new StubXpathModel
                {
                    QueryString = "/soap:Envelope/soap:Body/m:GetStockPrice/m:Description[text() = 'An umbrella']"
                }
            }
        };

        _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

        // act
        var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    [TestMethod]
    public void XPathConditionChecker_Validate_StubsFound_HappyFlow_WithoutNamespaces()
    {
        // arrange
        const string body = @"<?xml version=""1.0""?>
<object>
	<a>TEST</a>
	<b>TEST2</b>
</object>";
        var conditions = new StubConditionsModel
        {
            Xpath = new[]
            {
                new StubXpathModel
                {
                    QueryString = "/object/a[text() = 'TEST']"
                },
                new StubXpathModel
                {
                    QueryString = "/object/b[text() = 'TEST2']"
                }
            }
        };

        _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

        // act
        var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}