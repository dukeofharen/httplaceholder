using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Domain.Tests;

[TestClass]
public class StubResponseImageModelFacts
{
    [TestMethod]
    public void Hash_ShouldCalculateCorrectHash()
    {
        // Arrange
        var model = new StubResponseImageModel
        {
            Height = 512,
            Text = "Test text",
            Type = ResponseImageType.Jpeg,
            Width = 1024,
            BackgroundColor = "#FFFFFF",
            FontColor = "#000000",
            FontSize = 10,
            JpegQuality = 95,
            WordWrap = true
        };

        // Act / Assert
        Assert.AreEqual("9c2627f4f6cec8843c224a40fc4614a8", model.Hash);
    }

    [DataTestMethod]
    [DataRow(ResponseImageType.Jpeg, "image/jpeg")]
    [DataRow(ResponseImageType.Png, "image/png")]
    [DataRow(ResponseImageType.NotSet, "image/png")]
    [DataRow(null, "image/png")]
    [DataRow(ResponseImageType.Bmp, "image/bmp")]
    [DataRow(ResponseImageType.Gif, "image/gif")]
    public void ContentTypeHeaderValue_ShouldBeConvertedCorrectly(ResponseImageType? type, string mimeType)
    {
        // Arrange
        var model = new StubResponseImageModel {Type = type};

        // Act / Assert
        Assert.AreEqual(mimeType, model.ContentTypeHeaderValue);
    }
}
