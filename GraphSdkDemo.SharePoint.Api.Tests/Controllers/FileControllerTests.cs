using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GraphSdkDemo.SharePoint.Contracts.Services;
using GraphSdkDemo.Tests.Common.Constants;
using GraphSdkDemo.Tests.Common.Dependencies;
using GraphSdkDemo.SharePoint.Contracts.Files;
using GraphSdkDemo.Tests.Common.Dependencies.Enums;
using GraphSdkDemo.SharePoint.Api.Contracts.File;
using GraphSdkDemo.SharePoint.Api.Controllers;

namespace GraphSdkDemo.SharePoint.Api.Tests.Controllers
{
    public class FileControllerTests
    {
        private Mock<IFileService> _fileServiceMock;
        private Mock<ISettingsProvider> _settingsProviderMock;
        private FileController _controller;

        [SetUp]
        public void Initialize()
        {
            _fileServiceMock = new Mock<IFileService>();

            _settingsProviderMock = new Mock<ISettingsProvider>();
            _settingsProviderMock.Setup(x => x.GetSiteId()).Returns("siteId");

            _controller = new FileController(_fileServiceMock.Object, _settingsProviderMock.Object);
        }

        #region GetDocumentLibraries
        [Test]
        [Category(Categories.UnitTesting)]
        public async Task GetDocumentLibraries_Returns_DocumentLibraries_When_Found()
        {
            // Arrange
            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);
            var documentLibraries = new List<DocumentLibrary>
            {
                new DocumentLibrary{Id = "Id1", Name = "DocumentLibrary1"},
                new DocumentLibrary{Id = "Id2", Name = "DocumentLibrary2"},
            };
            _fileServiceMock.Setup(x => x.GetDocumentLibrariesAsync(It.Is<string>(p => p == "siteId"))).ReturnsAsync(DocumentLibrariesResult.Success(documentLibraries));

            // Act
            var httpActionResult = await _controller.GetDocumentLibraries(req, logger);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), httpActionResult);

            _fileServiceMock.Verify(x => x.GetDocumentLibrariesAsync(It.Is<string>(p => p == "siteId")), Times.Once);

            var result = ((OkObjectResult) httpActionResult).Value as DocumentLibraryModelInfo[];
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Id1", result[0].Id);
            Assert.AreEqual("DocumentLibrary1", result[0].Name);
            Assert.AreEqual("Id2", result[1].Id);
            Assert.AreEqual("DocumentLibrary2", result[1].Name);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task GetDocumentLibraries_Returns_EmptyArray_When_NoDocumentLibrariesFound()
        {
            // Arrange
            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);
            var documentLibraries = new List<DocumentLibrary>();
            _fileServiceMock.Setup(x => x.GetDocumentLibrariesAsync(It.Is<string>(p => p == "siteId"))).ReturnsAsync(DocumentLibrariesResult.Success(documentLibraries));

            // Act
            var httpActionResult = await _controller.GetDocumentLibraries(req, logger);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), httpActionResult);

            _fileServiceMock.Verify(x => x.GetDocumentLibrariesAsync(It.Is<string>(p => p == "siteId")), Times.Once);

            var result = ((OkObjectResult)httpActionResult).Value as DocumentLibraryModelInfo[];
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task GetDocumentLibraries_Returns_BadRequestObjectResult_When_AnErrorOccurred()
        {
            // Arrange
            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);
            _fileServiceMock.Setup(x => x.GetDocumentLibrariesAsync(It.Is<string>(p => p == "siteId"))).ReturnsAsync(DocumentLibrariesResult.Failed("An error occurred."));

            // Act
            var httpActionResult = await _controller.GetDocumentLibraries(req, logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);

            _fileServiceMock.Verify(x => x.GetDocumentLibrariesAsync(It.Is<string>(p => p == "siteId")), Times.Once);

            var result = ((BadRequestObjectResult)httpActionResult);
            Assert.IsNotNull(result);
            Assert.AreEqual("An error occurred.", result.Value);
        }
        #endregion

        #region GetFiles
        [Test]
        [Category(Categories.UnitTesting)]
        public async Task GetFiles_Returns_Files_When_Found()
        {
            // Arrange
            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);
            var documentLibraryId = "documentLibraryId1";
            var files = new List<File>
            {
                new File
                {
                    Id = "Id1",
                    Name = "FileName1",
                    Description = "FileDescription1",
                    CreatedAt = new DateTimeOffset(new DateTime(2020, 3, 4)),
                    LastModifiedAt = new DateTimeOffset(new DateTime(2020, 3, 4)),
                    DownloadUrl = "https://downloadurl1",
                    Size = 250L
                },
                new File
                {
                    Id = "Id2",
                    Name = "FileName2",
                    Description = "FileDescription2",
                    CreatedAt = new DateTimeOffset(new DateTime(2020, 3, 5)),
                    LastModifiedAt = new DateTimeOffset(new DateTime(2020, 3, 5)),
                    DownloadUrl = "https://downloadurl2",
                    Size = 500L
                }
            };
            _fileServiceMock.Setup(x => x.GetFilesAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId))).ReturnsAsync(FilesResult.Success(files));

            // Act
            var httpActionResult = await _controller.GetFiles(req, documentLibraryId, logger);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), httpActionResult);

            _fileServiceMock.Verify(x => x.GetFilesAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId)), Times.Once);

            var result = ((OkObjectResult)httpActionResult).Value as FileModelInfo[];
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Id1", result[0].Id);
            Assert.AreEqual("FileName1", result[0].Name);
            Assert.AreEqual("FileDescription1", result[0].Description);
            Assert.AreEqual(new DateTime(2020, 3, 4), result[0].CreatedAt);
            Assert.AreEqual(new DateTime(2020, 3, 4), result[0].LastModifiedAt);
            Assert.AreEqual("https://downloadurl1", result[0].DownloadUrl);
            Assert.AreEqual(250L, result[0].Size);
            Assert.AreEqual("Id2", result[1].Id);
            Assert.AreEqual("FileName2", result[1].Name);
            Assert.AreEqual("FileDescription2", result[1].Description);
            Assert.AreEqual(new DateTime(2020, 3, 5), result[1].CreatedAt);
            Assert.AreEqual(new DateTime(2020, 3, 5), result[1].LastModifiedAt);
            Assert.AreEqual("https://downloadurl2", result[1].DownloadUrl);
            Assert.AreEqual(500L, result[1].Size);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task GetFiles_Returns_EmptyArray_When_NoFilesFound()
        {
            // Arrange
            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);
            var documentLibraryId = "documentLibraryId1";
            var files = new List<File>();
            _fileServiceMock.Setup(x => x.GetFilesAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId))).ReturnsAsync(FilesResult.Success(files));

            // Act
            var httpActionResult = await _controller.GetFiles(req, documentLibraryId, logger);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), httpActionResult);

            _fileServiceMock.Verify(x => x.GetFilesAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId)), Times.Once);

            var result = ((OkObjectResult)httpActionResult).Value as FileModelInfo[];
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task GetFiles_Returns_BadRequestObjectResult_When_AnErrorOccurred()
        {
            // Arrange
            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            var documentLibraryId = "documentLibraryId1";
            _fileServiceMock.Setup(x => x.GetFilesAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId))).ReturnsAsync(FilesResult.Failed("An error occurred."));

            // Act
            var httpActionResult = await _controller.GetFiles(req, documentLibraryId, logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);

            _fileServiceMock.Verify(x => x.GetFilesAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId)), Times.Once);

            var result = ((BadRequestObjectResult)httpActionResult);
            Assert.IsNotNull(result);
            Assert.AreEqual("An error occurred.", result.Value);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task GetFiles_Returns_BadRequestObjectResult_When_DocumentLibraryIdIsNull()
        {
            // Arrange
            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            var httpActionResult = await _controller.GetFiles(req, null, logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);

            _fileServiceMock.Verify(x => x.GetFilesAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == null)), Times.Never);

            var result = ((BadRequestObjectResult)httpActionResult);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(SerializableError), result.Value);
            var errors = (SerializableError)result.Value;
            Assert.IsTrue(errors.ContainsKey("documentLibraryId"));
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task GetFiles_Returns_BadRequestObjectResult_When_DocumentLibraryIdIsEmpty()
        {
            // Arrange
            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);
            var documentLibraryId = string.Empty;

            // Act
            var httpActionResult = await _controller.GetFiles(req, documentLibraryId, logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);

            _fileServiceMock.Verify(x => x.GetFilesAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId)), Times.Never);

            var result = ((BadRequestObjectResult)httpActionResult);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(SerializableError), result.Value);
            var errors = (SerializableError)result.Value;
            Assert.IsTrue(errors.ContainsKey("documentLibraryId"));
        }
        #endregion

        #region Download
        [Test]
        [Category(Categories.UnitTesting)]
        public async Task Download_Returns_File_When_Found()
        {
            // Arrange
            var documentLibraryId = "b!Uh445jcGFkK0-jWspn-lx74bPae4-DJEiIWsqKfPgGyQV63S1iPeRIZxnI5dGymZ";
            var driveItemId = "01FG5K4LMYZK6PXIJOOFG3LDYWNPSSFMLR";

            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);
            _fileServiceMock.Setup(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(x => x == documentLibraryId), It.Is<string>(x => x == driveItemId))).ReturnsAsync(DownloadResult.Success(driveItemId, "MyDocument.docx", new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }));

            // Act
            var httpActionResult = await _controller.Download(req, documentLibraryId, driveItemId, logger);

            // Assert
            _fileServiceMock.Verify(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(x => x == documentLibraryId), It.Is<string>(x => x == driveItemId)), Times.Once);
            Assert.IsInstanceOf(typeof(FileContentResult), httpActionResult);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task Download_Returns_BadRequest_When_DocumentLibraryIdIsNull()
        {
            // Arrange
            string documentLibraryId = null;
            var driveItemId = "01FG5K4LMYZK6PXIJOOFG3LDYWNPSSFMLR";

            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            var httpActionResult = await _controller.Download(req, documentLibraryId, driveItemId, logger);

            // Assert
            _fileServiceMock.Verify(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId), It.Is<string>(p => p == driveItemId)), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task Download_Returns_BadRequest_When_DocumentLibraryIdIsEmpty()
        {
            // Arrange
            var documentLibraryId = string.Empty;
            var driveItemId = "01FG5K4LMYZK6PXIJOOFG3LDYWNPSSFMLR";

            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            var httpActionResult = await _controller.Download(req, documentLibraryId, driveItemId, logger);

            // Assert
            _fileServiceMock.Verify(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId), It.Is<string>(p => p == driveItemId)), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task Download_Returns_BadRequest_When_DriveItemIdIsNull()
        {
            // Arrange
            var documentLibraryId = "b!Uh445jcGFkK0-jWspn-lx74bPae4-DJEiIWsqKfPgGyQV63S1iPeRIZxnI5dGymZ";
            string driveItemId = null;

            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            var httpActionResult = await _controller.Download(req, documentLibraryId, driveItemId, logger);

            // Assert
            _fileServiceMock.Verify(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId), It.Is<string>(p => p == driveItemId)), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task Download_Returns_BadRequest_When_DriveItemIdIsEmpty()
        {
            // Arrange
            var documentLibraryId = "b!Uh445jcGFkK0-jWspn-lx74bPae4-DJEiIWsqKfPgGyQV63S1iPeRIZxnI5dGymZ";
            var driveItemId = string.Empty;

            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            var httpActionResult = await _controller.Download(req, documentLibraryId, driveItemId, logger);

            // Assert
            _fileServiceMock.Verify(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId), It.Is<string>(p => p == driveItemId)), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }
        #endregion

        #region DownloadMultiple
        [Test]
        [Category(Categories.UnitTesting)]
        public async Task DownloadMultiple_Returns_Files_When_Found()
        {
            // Arrange
            var documentLibraryId = "DL1";
            var body = "{\"driveItems\": [\"DI1\", \"DI2\"]}";
            var reqMock = TestFactory.CreateHttpRequestMock(body);
            var logger = TestFactory.CreateLogger(LoggerTypes.List);
            _fileServiceMock.Setup(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(x => x == documentLibraryId), It.Is<string>(x => x == "DI1"))).ReturnsAsync(DownloadResult.Success("DI1", "File1.docx", new byte[] { 0x01, 0x01, 0x01 }));
            _fileServiceMock.Setup(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(x => x == documentLibraryId), It.Is<string>(x => x == "DI2"))).ReturnsAsync(DownloadResult.Success("DI2", "File2.docx", new byte[] { 0x02, 0x02, 0x02 }));
            
            // Act
            var httpActionResult = await _controller.DownloadMultiple(reqMock.Object, documentLibraryId, logger);

            // Assert
            _fileServiceMock.Verify(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(x => x == documentLibraryId), It.Is<string>(x => x == "DI1")), Times.Once);
            _fileServiceMock.Verify(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(x => x == documentLibraryId), It.Is<string>(x => x == "DI2")), Times.Once);
            Assert.IsInstanceOf(typeof(FileContentResult), httpActionResult);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task DownloadMultiple_Returns_BadRequest_When_DocumentLibraryIdIsNull()
        {
            // Arrange
            string documentLibraryId = null;
            var body = "{\"driveItems\": [\"DI1\", \"DI2\"]}";
            var reqMock = TestFactory.CreateHttpRequestMock(body);
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            var httpActionResult = await _controller.DownloadMultiple(reqMock.Object, documentLibraryId, logger);

            // Assert
            _fileServiceMock.Verify(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId), It.IsAny<string>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task DownloadMultiple_Returns_BadRequest_When_DocumentLibraryIdIsEmpty()
        {
            // Arrange
            var documentLibraryId = string.Empty;
            var body = "{\"driveItems\": [\"DI1\", \"DI2\"]}";
            var reqMock = TestFactory.CreateHttpRequestMock(body);
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            var httpActionResult = await _controller.DownloadMultiple(reqMock.Object, documentLibraryId, logger);

            // Assert
            _fileServiceMock.Verify(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId), It.IsAny<string>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task DownloadMultiple_Returns_BadRequest_When_DriveItemIdIsNull()
        {
            // Arrange
            var documentLibraryId = "DL1";
            var body = "{\"driveItems\": [null]}";
            var reqMock = TestFactory.CreateHttpRequestMock(body);
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            var httpActionResult = await _controller.DownloadMultiple(reqMock.Object, documentLibraryId, logger);

            // Assert
            _fileServiceMock.Verify(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId), It.IsAny<string>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }

        [Test]
        [Category(Categories.UnitTesting)]
        public async Task DownloadMultiple_Returns_BadRequest_When_DriveItemIdIsEmpty()
        {
            // Arrange
            var documentLibraryId = "DL1";
            var body = "{\"driveItems\": [\"\"]}";
            var reqMock = TestFactory.CreateHttpRequestMock(body);
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            var httpActionResult = await _controller.DownloadMultiple(reqMock.Object, documentLibraryId, logger);

            // Assert
            _fileServiceMock.Verify(x => x.DownloadAsync(It.Is<string>(p => p == "siteId"), It.Is<string>(p => p == documentLibraryId), It.IsAny<string>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }
        #endregion
    }
}
