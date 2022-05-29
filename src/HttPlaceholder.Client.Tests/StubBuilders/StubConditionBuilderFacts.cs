using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.StubBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Client.Tests.StubBuilders;

[TestClass]
public class StubConditionBuilderFacts
{
    [TestMethod]
    public void WithHttpMethodAsString()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithHttpMethod("POST")
            .Build();

        // Assert
        Assert.AreEqual("POST", conditions.Method);
    }

    [TestMethod]
    public void WithHttpMethodAsHttpMethod()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithHttpMethod(HttpMethod.Post)
            .Build();

        // Assert
        Assert.AreEqual("POST", conditions.Method);
    }

    [TestMethod]
    public void WithPath()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithPath("/path")
            .Build();

        // Assert
        Assert.AreEqual("/path", conditions.Url.Path);
    }

    [TestMethod]
    public void WithPathDto()
    {
        // Act
        var dto = new StubConditionStringCheckingDto {StringEquals = "/users"};
        var conditions = StubConditionBuilder.Begin()
            .WithPath(dto)
            .Build();

        // Assert
        Assert.AreEqual(dto, conditions.Url.Path);
    }

    [TestMethod]
    public void WithPathBuilder()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithPath(StringCheckingDtoBuilder.Begin().StringEquals("/users"))
            .Build();

        // Assert
        Assert.AreEqual("/users", ((StubConditionStringCheckingDto)conditions.Url.Path).StringEquals);
    }

    [TestMethod]
    public void WithQueryStringParameter()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithQueryStringParameter("q1", "val1")
            .WithQueryStringParameter("q2", "val2")
            .Build();

        // Assert
        Assert.AreEqual(2, conditions.Url.Query.Count);
        Assert.AreEqual("val1", conditions.Url.Query["q1"]);
        Assert.AreEqual("val2", conditions.Url.Query["q2"]);
    }

    [TestMethod]
    public void WithQueryStringParameterDto()
    {
        // Act
        var dto1 = new StubConditionStringCheckingDto {StringEquals = "val1"};
        var dto2 = new StubConditionStringCheckingDto {StringEquals = "val1"};

        var conditions = StubConditionBuilder.Begin()
            .WithQueryStringParameter("q1", dto1)
            .WithQueryStringParameter("q2", dto2)
            .Build();

        // Assert
        Assert.AreEqual(2, conditions.Url.Query.Count);
        Assert.AreEqual(dto1, conditions.Url.Query["q1"]);
        Assert.AreEqual(dto2, conditions.Url.Query["q2"]);
    }

    [TestMethod]
    public void WithQueryStringParameterBuilder()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithQueryStringParameter("q1", StringCheckingDtoBuilder.Begin().StringEquals("val1"))
            .WithQueryStringParameter("q2", StringCheckingDtoBuilder.Begin().StringEquals("val2"))
            .Build();

        // Assert
        Assert.AreEqual(2, conditions.Url.Query.Count);
        Assert.AreEqual("val1", ((StubConditionStringCheckingDto)conditions.Url.Query["q1"]).StringEquals);
        Assert.AreEqual("val2", ((StubConditionStringCheckingDto)conditions.Url.Query["q2"]).StringEquals);
    }

    [TestMethod]
    public void WithFullPath()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithFullPath("/full-path")
            .Build();

        // Assert
        Assert.AreEqual("/full-path", conditions.Url.FullPath);
    }

    [TestMethod]
    public void WithFullPathDto()
    {
        // Act
        var dto = new StubConditionStringCheckingDto {StringEquals = "/users"};
        var conditions = StubConditionBuilder.Begin()
            .WithFullPath(dto)
            .Build();

        // Assert
        Assert.AreEqual(dto, conditions.Url.FullPath);
    }

    [TestMethod]
    public void WithFullPathBuilder()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithFullPath(StringCheckingDtoBuilder.Begin().StringEquals("/users"))
            .Build();

        // Assert
        Assert.AreEqual("/users", ((StubConditionStringCheckingDto)conditions.Url.FullPath).StringEquals);
    }

    [TestMethod]
    public void WithHttpsEnabled()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithHttpsEnabled()
            .Build();

        // Assert
        Assert.IsTrue(conditions.Url.IsHttps);
    }

    [TestMethod]
    public void WithHttpsDisabled()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithHttpsDisabled()
            .Build();

        // Assert
        Assert.IsFalse(conditions.Url.IsHttps);
    }

    [TestMethod]
    public void WithPostedBodySubstring()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithPostedBodySubstring("val1")
            .WithPostedBodySubstring("val2")
            .Build();

        // Assert
        Assert.AreEqual(2, conditions.Body.Count());
        Assert.AreEqual("val1", conditions.Body.ElementAt(0));
        Assert.AreEqual("val2", conditions.Body.ElementAt(1));
    }

    [TestMethod]
    public void WithPostedFormValue()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithPostedFormValue("form1", "val1")
            .WithPostedFormValue("form2", "val2")
            .Build();

        // Assert
        Assert.AreEqual(2, conditions.Form.Count());

        var form1 = conditions.Form.ElementAt(0);
        Assert.AreEqual("form1", form1.Key);
        Assert.AreEqual("val1", form1.Value);

        var form2 = conditions.Form.ElementAt(1);
        Assert.AreEqual("form2", form2.Key);
        Assert.AreEqual("val2", form2.Value);
    }

    [TestMethod]
    public void WithRequestHeader()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithRequestHeader("X-Header-1", "val1")
            .WithRequestHeader("X-Header-2", "val2")
            .Build();

        // Assert
        Assert.AreEqual(2, conditions.Headers.Count);
        Assert.AreEqual("val1", conditions.Headers["X-Header-1"]);
        Assert.AreEqual("val2", conditions.Headers["X-Header-2"]);
    }

    [TestMethod]
    public void WithXPathCondition()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithXPathCondition("XPathQuery1", new Dictionary<string, string> {{"soap1", "soap namespace1"}})
            .WithXPathCondition("XPathQuery2", new Dictionary<string, string> {{"soap2", "soap namespace2"}})
            .Build();

        // Assert
        Assert.AreEqual(2, conditions.Xpath.Count());

        var xpath1 = conditions.Xpath.ElementAt(0);
        Assert.AreEqual("XPathQuery1", xpath1.QueryString);
        Assert.AreEqual("soap namespace1", xpath1.Namespaces["soap1"]);

        var xpath2 = conditions.Xpath.ElementAt(1);
        Assert.AreEqual("XPathQuery2", xpath2.QueryString);
        Assert.AreEqual("soap namespace2", xpath2.Namespaces["soap2"]);
    }

    [TestMethod]
    public void WithJsonPathCondition()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithJsonPathCondition("jsonpath1")
            .WithJsonPathCondition("jsonpath2", "value2")
            .Build();

        // Assert
        Assert.AreEqual(2, conditions.JsonPath.Count());

        Assert.AreEqual("jsonpath1", conditions.JsonPath.ElementAt(0));

        var stubJsonPathModel = (StubJsonPathModel)conditions.JsonPath.ElementAt(1);
        Assert.AreEqual("jsonpath2", stubJsonPathModel.Query);
        Assert.AreEqual("value2", stubJsonPathModel.ExpectedValue);
    }

    [TestMethod]
    public void WithBasicAuthentication()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithBasicAuthentication("username", "password")
            .Build();

        // Assert
        Assert.AreEqual("username", conditions.BasicAuthentication.Username);
        Assert.AreEqual("password", conditions.BasicAuthentication.Password);
    }

    [TestMethod]
    public void WithClientIp()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithClientIp("11.22.33.44")
            .Build();

        // Assert
        Assert.AreEqual("11.22.33.44", conditions.ClientIp);
    }

    [TestMethod]
    public void WithIpInBlock()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithIpInBlock("127.0.0.0", "30")
            .Build();

        // Assert
        Assert.AreEqual("127.0.0.0/30", conditions.ClientIp);
    }

    [TestMethod]
    public void WithHost()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .WithHost("httplaceholder.com")
            .Build();

        // Assert
        Assert.AreEqual("httplaceholder.com", conditions.Host);
    }

    [TestMethod]
    public void WithJsonObject()
    {
        // Act
        var obj = new {key = "val"};
        var conditions = StubConditionBuilder.Begin()
            .WithJsonObject(obj)
            .Build();

        // Assert
        Assert.AreEqual(obj, conditions.Json);
    }

    [TestMethod]
    public void WithJsonArray()
    {
        // Act
        var array = new[] {new {key = "val1"}, new {key = "val2"}};
        var conditions = StubConditionBuilder.Begin()
            .WithJsonArray(array)
            .Build();

        // Assert
        Assert.AreEqual(array, conditions.Json);
    }

    [TestMethod]
    public void ScenarioHasAtLeastXHits()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .ScenarioHasAtLeastXHits(2)
            .Build();

        // Assert
        Assert.AreEqual(2, conditions.Scenario.MinHits);
    }

    [TestMethod]
    public void ScenarioHasAtMostXHits()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .ScenarioHasAtMostXHits(2)
            .Build();

        // Assert
        Assert.AreEqual(2, conditions.Scenario.MaxHits);
    }

    [TestMethod]
    public void ScenarioHasExactlyXHits()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .ScenarioHasExactlyXHits(2)
            .Build();

        // Assert
        Assert.AreEqual(2, conditions.Scenario.ExactHits);
    }

    [TestMethod]
    public void ScenarioHasState()
    {
        // Act
        var conditions = StubConditionBuilder.Begin()
            .ScenarioHasState("new-state")
            .Build();

        // Assert
        Assert.AreEqual("new-state", conditions.Scenario.ScenarioState);
    }
}
