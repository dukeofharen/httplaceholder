using AutoMapper;
using HttPlaceholder.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class AutoMapperExtensionsFacts
{
    [TestMethod]
    public void MapAndSet_HappyFlow()
    {
        // Arrange
        var mapperMock = new Mock<IMapper>();
        var mappedModel = new TestModel();
        var input = new object();
        mapperMock
            .Setup(m => m.Map<TestModel>(input))
            .Returns(mappedModel);

        // Act
        var result = mapperMock.Object.MapAndSet<TestModel>(input, m => m.Value = "test123");

        // Assert
        Assert.AreEqual("test123", mappedModel.Value);
    }

    private class TestModel
    {
        public string Value { get; set; }
    }
}
