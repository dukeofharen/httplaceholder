﻿using System.IO;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriting
{
    [TestClass]
    public class FileResponseWriterFacts
    {
        private readonly Mock<IFileService> _fileServiceMock = new Mock<IFileService>();
        private readonly Mock<IStubRootPathResolver> _stubRootPathResolverMock = new Mock<IStubRootPathResolver>();
        private FileResponseWriter _writer;

        [TestInitialize]
        public void Initialize() =>
            _writer = new FileResponseWriter(
                _fileServiceMock.Object,
                _stubRootPathResolverMock.Object);

        [TestCleanup]
        public void Cleanup()
        {
            _fileServiceMock.VerifyAll();
            _stubRootPathResolverMock.VerifyAll();
        }

        [TestMethod]
        public async Task FileResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    File = null
                }
            };

            var response = new ResponseModel();

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsFalse(result.Executed);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public async Task FileResponseWriter_WriteToResponseAsync_HappyFlow_FileFoundDirectly()
        {
            // arrange
            var body = new byte[] { 1, 2, 3 };
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    File = @"C:\tmp\image.png"
                }
            };

            var response = new ResponseModel();

            _fileServiceMock
               .Setup(m => m.FileExists(stub.Response.File))
               .Returns(true);

            _fileServiceMock
               .Setup(m => m.ReadAllBytes(stub.Response.File))
               .Returns(body);

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result.Executed);
            Assert.AreEqual(body, response.Body);
        }

        [TestMethod]
        public async Task FileResponseWriter_WriteToResponseAsync_HappyFlow_FileNotFoundDirectly_ButFoundInStubFolder()
        {
            // arrange
            var stubRootPaths = new[] {"/var/stubs1", "/var/stubs2"};
            const string file = "image.png";
            var expectedFolder = Path.Combine(stubRootPaths[1], file);
            var body = new byte[] { 1, 2, 3 };
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    File = file
                }
            };

            var response = new ResponseModel();

            _stubRootPathResolverMock
               .Setup(m => m.GetStubRootPaths())
               .Returns(stubRootPaths);

            _fileServiceMock
               .Setup(m => m.FileExists(stub.Response.File))
               .Returns(false);

            _fileServiceMock
               .Setup(m => m.FileExists(expectedFolder))
               .Returns(true);

            _fileServiceMock
               .Setup(m => m.ReadAllBytes(expectedFolder))
               .Returns(body);

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result.Executed);
            Assert.AreEqual(body, response.Body);
        }

        [TestMethod]
        public async Task FileResponseWriter_WriteToResponseAsync_FileNotFoundDirectly_AlsoNotFoundInStubFolder_ShouldReturnNoBody()
        {
            // arrange
            const string file = "image.png";
            var stubRootPaths = new[] {"/var/stubs1", "/var/stubs2"};
            var expectedFolder = Path.Combine(stubRootPaths[0], file);
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    File = file
                }
            };

            var response = new ResponseModel();

            _stubRootPathResolverMock
               .Setup(m => m.GetStubRootPaths())
               .Returns(stubRootPaths);

            _fileServiceMock
               .Setup(m => m.FileExists(stub.Response.File))
               .Returns(false);

            _fileServiceMock
               .Setup(m => m.FileExists(expectedFolder))
               .Returns(false);

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsFalse(result.Executed);
            Assert.IsNull(response.Body);
        }
    }
}
