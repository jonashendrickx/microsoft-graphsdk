using System.Threading.Tasks;
using GraphSdkDemo.SharePoint.Contracts.Enums;
using GraphSdkDemo.SharePoint.Contracts.Services;
using GraphSdkDemo.Tests.Common.Constants;
using GraphSdkDemo.Tests.Common.Helpers;
using Moq;
using NUnit.Framework;

namespace GraphSdkDemo.SharePoint.Services.IntegrationTests
{
    public class FileServiceIntegrationTests
    {
        private readonly Mock<ISettingsProvider> _settingsProviderMock = new Mock<ISettingsProvider>();
        private FileService _service;

        [OneTimeSetUp]
        public void InitializeOnce()
        {
            _settingsProviderMock.Setup(x => x.GetClientId()).Returns(SettingsHelper.ClientId);
            _settingsProviderMock.Setup(x => x.GetTenantId()).Returns(SettingsHelper.TenantId);
            _settingsProviderMock.Setup(x => x.GetSiteId()).Returns(SettingsHelper.SiteId);
            _settingsProviderMock.Setup(x => x.GetServiceAccountUsername()).Returns(SettingsHelper.ServiceAccountUsername);
            _settingsProviderMock.Setup(x => x.GetServiceAccountPassword()).Returns(SettingsHelper.ServiceAccountPassword);
        }

        [SetUp]
        public void Initialize()
        {
            _service = new FileService(_settingsProviderMock.Object);
        }

        #region GetDocumentLibrariesAsync
        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task GetDocumentLibrariesAsync_Returns_Success_When_Found()
        {
            // Arrange

            // Act
            var result = await _service.GetDocumentLibrariesAsync(SettingsHelper.SiteId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ConnectionStatus.Success, result.Status);
        }
        #endregion

        #region GetFilesAsync
        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task GetFilesAsync_Returns_Success_When_Found()
        {
            // Arrange

            // Act
            var result = await _service.GetFilesAsync(SettingsHelper.SiteId, SettingsHelper.DocumentLibraryId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ConnectionStatus.Success, result.Status);
        }

        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task GetFilesAsync_Returns_Failed_When_NotFound()
        {
            // Arrange

            // Act
            var result = await _service.GetFilesAsync(SettingsHelper.SiteId, "wrong-id");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ConnectionStatus.Error, result.Status);
        }
        #endregion

        #region DownloadAsync
        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task DownloadAsync_Returns_Success_When_Found()
        {
            // Arrange

            // Act
            var result = await _service.DownloadAsync(SettingsHelper.SiteId, SettingsHelper.DocumentLibraryId, SettingsHelper.DriveItemId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, "LinkTest1.docx");
            Assert.AreEqual(result.Id, SettingsHelper.DriveItemId);
            Assert.IsNotNull(result.Content);
        }

        [Test]
        [Category(Categories.IntegrationTesting)]
        public async Task DownloadAsync_Returns_Error_When_Found()
        {
            // Arrange

            // Act
            var result = await _service.DownloadAsync(SettingsHelper.SiteId, "wrong-id", "wrong-id");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ConnectionStatus.Error, result.Status);
        }
        #endregion
    }
}
