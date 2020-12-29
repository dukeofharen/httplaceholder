using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Domain.Tests
{
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
                Type = "jpeg",
                Width = 1024,
                BackgroundColor = "#FFFFFF",
                FontColor = "#000000",
                FontSize = 10,
                JpegQuality = 95,
                WordWrap = true
            };

            // Act / Assert
            Assert.AreEqual("f015841bcda9c4b75cdbe54795251bfa", model.Hash);
        }

        [DataTestMethod]
        [DataRow("jpeg", "image/jpeg")]
        [DataRow("png", "image/png")]
        [DataRow("bmp", "image/bmp")]
        [DataRow("gif", "image/gif")]
        public void ContentTypeHeaderValue_ShouldBeConvertedCorrectly(string type, string mimeType)
        {
            // Arrange
            var model = new StubResponseImageModel {Type = type};

            // Act / Assert
            Assert.AreEqual(mimeType, model.ContentTypeHeaderValue);
        }
    }
}
