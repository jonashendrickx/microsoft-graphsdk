using GraphSdkDemo.SharePoint.Api.IntegrationTests.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GraphSdkDemo.SharePoint.Api;
using GraphSdkDemo.Tests.Common.Helpers;
using GraphSdkDemo.Tests.Common.Dependencies;
using GraphSdkDemo.Tests.Common.Constants;
using GraphSdkDemo.Tests.Common.Dependencies.Enums;
using GraphSdkDemo.SharePoint.Contracts.Services;
using GraphSdkDemo.SharePoint.Api.Contracts.File;
using GraphSdkDemo.SharePoint.Api.Controllers;

namespace GraphSdkDemo.SharePoint.Api.IntegrationTests.Controllers
{
    public class FileControllerIntegrationTests
    {
        private readonly FileController _sut;
        private readonly ILogger _logger;

        public FileControllerIntegrationTests()
        {
            var host = new TestHost();
            var settingsProviderMock = new SettingsProviderMock();
            _sut = new FileController(host.ServiceProvider.GetRequiredService<IFileService>(), settingsProviderMock);
            _logger = host.ServiceProvider.GetRequiredService<ILogger>();
        }

        #region GetDocumentLibraries
        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task GetDocumentLibraries_Returns_Ok_When_Found()
        {
            // Arrange
            var req = TestFactory.CreateHttpRequest();

            // Act
            var httpActionResult = await _sut.GetDocumentLibraries(req, _logger);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), httpActionResult);

            var result = ((OkObjectResult)httpActionResult).Value as DocumentLibraryModelInfo[];
            Assert.IsNotNull(result);
        }
        #endregion

        #region GetFilesAsync
        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task GetFilesAsync_Returns_Ok_When_Found()
        {
            // Arrange
            var req = new DefaultHttpRequest(new DefaultHttpContext());

            // Act
            var httpActionResult = await _sut.GetFiles(req, SettingsHelper.DocumentLibraryId, _logger);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), httpActionResult);
            var result = ((OkObjectResult)httpActionResult).Value as FileModelInfo[];
            
            Assert.IsNotNull(result);
        }

        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task GetFilesAsync_Returns_BadRequest_When_NotFound()
        {
            // Arrange
            var req = new DefaultHttpRequest(new DefaultHttpContext());

            // Act
            var httpActionResult = await _sut.GetFiles(req, "wrong-id", _logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
            var result = ((BadRequestObjectResult)httpActionResult).Value;
            Assert.IsNotNull(result);

        }
        #endregion

        #region Download
        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task Download_Returns_File_When_Found()
        {
            // Arrange
            var req = TestFactory.CreateHttpRequest();

            var documentLibraryId = SettingsHelper.DocumentLibraryId;
            var driveItemId = SettingsHelper.DriveItemId;

            // Act
            var httpActionResult = await _sut.Download(req, documentLibraryId, driveItemId, _logger);

            // Assert
            Assert.IsInstanceOf(typeof(FileContentResult), httpActionResult);

        }

        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task Download_Returns_BadRequest_When_DocumentLibraryIdIsNull()
        {
            // Arrange
            var req = TestFactory.CreateHttpRequest();

            var driveItemId = SettingsHelper.DriveItemId;

            // Act
            var httpActionResult = await _sut.Download(req, null, driveItemId, _logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }

        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task Download_Returns_BadRequest_When_DocumentLibraryIdIsEmpty()
        {
            // Arrange
            var req = TestFactory.CreateHttpRequest();

            var documentLibraryId = string.Empty;
            var driveItemId = SettingsHelper.DriveItemId;

            // Act
            var httpActionResult = await _sut.Download(req, documentLibraryId, driveItemId, _logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }

        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task Download_Returns_BadRequest_When_DriveItemIdIsNull()
        {
            // Arrange
            var documentLibraryId = SettingsHelper.DocumentLibraryId;

            var req = TestFactory.CreateHttpRequest();
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            var httpActionResult = await _sut.Download(req, documentLibraryId, null, logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }

        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task Download_Returns_BadRequest_When_DriveItemIdIsEmpty()
        {
            // Arrange
            var documentLibraryId = SettingsHelper.DocumentLibraryId;
            var driveItemId = string.Empty;

            var req = TestFactory.CreateHttpRequest();

            // Act
            var httpActionResult = await _sut.Download(req, documentLibraryId, driveItemId, _logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }
        #endregion

        #region DownloadMultiple
        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task DownloadMultiple_Returns_Files_When_Found()
        {
            // Arrange
            var body = "{\"driveItems\": [\"01EIR2WX4I7ZPREEQDMZG3SNTMBTLFBTID\", \"01EIR2WX4I7ZPREEQDMZG3SNTMBTLFBTID\"]}";

            var reqMock = TestFactory.CreateHttpRequestMock(body);
            
            var documentLibraryId = SettingsHelper.DocumentLibraryId;

            // Act
            var httpActionResult = await _sut.DownloadMultiple(reqMock.Object, documentLibraryId, _logger);

            // Assert
            Assert.IsInstanceOf(typeof(FileContentResult), httpActionResult);
        }

        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task DownloadMultiple_Returns_BadRequest_When_DocumentLibraryIdIsNull()
        {
            // Arrange
            var body = "{\"driveItems\": [\"01EIR2WX4I7ZPREEQDMZG3SNTMBTLFBTID\", \"01EIR2WX4I7ZPREEQDMZG3SNTMBTLFBTID\"]}";
            var reqMock = TestFactory.CreateHttpRequestMock(body);

            // Act
            var httpActionResult = await _sut.DownloadMultiple(reqMock.Object, null, _logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }

        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task DownloadMultiple_Returns_BadRequest_When_DocumentLibraryIdIsEmpty()
        {
            // Arrange
            var body = "{\"driveItems\": [\"01EIR2WX4I7ZPREEQDMZG3SNTMBTLFBTID\", \"01EIR2WX4I7ZPREEQDMZG3SNTMBTLFBTID\"]}";
            var reqMock = TestFactory.CreateHttpRequestMock(body);

            var documentLibraryId = string.Empty;

            // Act
            var httpActionResult = await _sut.DownloadMultiple(reqMock.Object, documentLibraryId, _logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }

        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task DownloadMultiple_Returns_BadRequest_When_DriveItemIdIsNull()
        {
            // Arrange
            var documentLibraryId = SettingsHelper.DocumentLibraryId;
            var body = "{\"driveItems\": [null]}";
            var reqMock = TestFactory.CreateHttpRequestMock(body);
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            var httpActionResult = await _sut.DownloadMultiple(reqMock.Object, documentLibraryId, logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }

        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task DownloadMultiple_Returns_BadRequest_When_DriveItemIdIsEmpty()
        {
            // Arrange
            var documentLibraryId = SettingsHelper.DocumentLibraryId;
            var driveItemId = string.Empty;

            var body = "{\"driveItems\": [\"\"]}";
            var reqMock = TestFactory.CreateHttpRequestMock(body);

            // Act
            var httpActionResult = await _sut.Download(reqMock.Object, documentLibraryId, driveItemId, _logger);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), httpActionResult);
        }
        #endregion
    }
}
