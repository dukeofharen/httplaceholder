﻿using System;
using System.IO;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SixLabors.ImageSharp;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriting
{
    [TestClass]
    public class ImageResponseWriterFacts
    {
        private static readonly string _executingAssemblyPath =
            OperatingSystem.IsWindows() ? @"C:\bin\httplaceholder" : "/bin/httplaceholder";

        private static readonly string _tempFolder = OperatingSystem.IsWindows() ? @"C:\temp" : "/tmp";

        private readonly Mock<IAssemblyService> _mockAssemblyService = new Mock<IAssemblyService>();
        private readonly Mock<IFileService> _mockFileService = new Mock<IFileService>();
        private ImageResponseWriter _responseWriter;

        [TestInitialize]
        public void Initialize()
        {
            _mockAssemblyService
                .Setup(m => m.GetExecutingAssemblyRootPath())
                .Returns(_executingAssemblyPath);
            _mockFileService
                .Setup(m => m.GetTempPath())
                .Returns(_tempFolder);
            _responseWriter = new ImageResponseWriter(_mockAssemblyService.Object, _mockFileService.Object);
        }

        [TestMethod]
        public async Task WriteToResponseAsync_StubNotConfigured_ShouldReturnNotExecuted()
        {
            // Arrange
            var stub = new StubModel {Response = new StubResponseModel {Image = null}};
            var response = new ResponseModel();

            // Act
            var result = await _responseWriter.WriteToResponseAsync(stub, response);

            // Assert
            Assert.IsFalse(result.Executed);
            Assert.AreEqual(0, response.Headers.Count);
            Assert.AreEqual(null, response.Body);
        }

        [TestMethod]
        public async Task WriteToResponseAsync_FileIsCached_ShouldReturnCachedFile()
        {
            // Arrange
            var stub = new StubModel {Response = new StubResponseModel {Image = new StubResponseImageModel
            {
                Type = ResponseImageType.Jpeg,
                Height = 512,
                Width = 512
            }}};
            var response = new ResponseModel();

            var cachedBytes = new byte[] {1, 2, 3, 4};
            var expectedCachePath = Path.Combine(_tempFolder, $"{stub.Response.Image.Hash}.bin");
            _mockFileService
                .Setup(m => m.FileExists(expectedCachePath))
                .Returns(true);
            _mockFileService
                .Setup(m => m.ReadAllBytes(expectedCachePath))
                .Returns(cachedBytes);

            // Act
            var result = await _responseWriter.WriteToResponseAsync(stub, response);

            // Assert
            Assert.IsTrue(result.Executed);
            Assert.AreEqual(1, response.Headers.Count);
            Assert.AreEqual(cachedBytes, response.Body);
        }

        [DataTestMethod]
        [DataRow(ResponseImageType.Jpeg, "image/jpeg")]
        [DataRow(ResponseImageType.Png, "image/png")]
        [DataRow(ResponseImageType.Bmp, "image/bmp")]
        [DataRow(ResponseImageType.Gif, "image/gif")]
        public async Task WriteToResponseAsync_AllFileTypes(ResponseImageType? type, string expectedContentType)
        {
            // Arrange
            var stub = new StubModel {Response = new StubResponseModel {Image = new StubResponseImageModel
            {
                Type = type,
                Height = 512,
                Width = 512
            }}};
            var response = new ResponseModel();

            _mockAssemblyService
                .Setup(m => m.GetExecutingAssemblyRootPath())
                .Returns(AssemblyHelper.GetExecutingAssemblyRootPath);

            var expectedCachePath = Path.Combine(_tempFolder, $"{stub.Response.Image.Hash}.bin");
            _mockFileService
                .Setup(m => m.FileExists(expectedCachePath))
                .Returns(false);

            // Act
            var result = await _responseWriter.WriteToResponseAsync(stub, response);

            // Assert
            Assert.IsTrue(result.Executed);
            Assert.AreEqual(1, response.Headers.Count);
            await using var ms = new MemoryStream(response.Body);
            using var image = await Image.LoadAsync(ms);
            Assert.AreEqual(stub.Response.Image.Height, image.Height);
            Assert.AreEqual(stub.Response.Image.Width, image.Width);
            Assert.AreEqual(expectedContentType, response.Headers["Content-Type"]);
            _mockFileService
                .Verify(m => m.WriteAllBytes(expectedCachePath, response.Body), Times.Once);
        }
    }
}
