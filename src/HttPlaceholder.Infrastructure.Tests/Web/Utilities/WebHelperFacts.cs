using HttPlaceholder.Infrastructure.Web.Utilities;
using HttPlaceholder.TestUtilities.Http;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Infrastructure.Tests.Web.Utilities;

[TestClass]
public class WebHelperFacts
{
    [TestMethod]
    public void GetHttpContext_HttpContextAccessorNull_ShouldThrowInvalidOperationException()
    {
        // Act
        var exception =
            Assert.ThrowsException<InvalidOperationException>(() => ((IHttpContextAccessor)null).GetHttpContext());

        // Assert
        Assert.AreEqual("HttpContext not set.", exception.Message);
    }

    [TestMethod]
    public void GetHttpContext_HttpContextNull_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock
            .Setup(m => m.HttpContext)
            .Returns((HttpContext)null);

        // Act
        var exception =
            Assert.ThrowsException<InvalidOperationException>(() => accessorMock.Object.GetHttpContext());

        // Assert
        Assert.AreEqual("HttpContext not set.", exception.Message);
    }

    [TestMethod]
    public void GetHttpContext_HappyFlow()
    {
        // Arrange
        var httpContext = new MockHttpContext();

        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock
            .Setup(m => m.HttpContext)
            .Returns(httpContext);

        // Act
        var result = accessorMock.Object.GetHttpContext();

        // Assert
        Assert.AreEqual(httpContext, result);
    }
}
