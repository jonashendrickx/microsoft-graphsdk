using GraphSdkDemo.SharePoint.Contracts.Services;
using GraphSdkDemo.Tests.Common.Helpers;

namespace GraphSdkDemo.SharePoint.Api.IntegrationTests.Dependencies
{
    public class SettingsProviderMock : ISettingsProvider
    {
        public string GetClientId() => SettingsHelper.ClientId;

        public string GetTenantId() => SettingsHelper.TenantId;
        public string GetSiteId() => SettingsHelper.SiteId;

        public string GetServiceAccountUsername() => SettingsHelper.ServiceAccountUsername;

        public string GetServiceAccountPassword() => SettingsHelper.ServiceAccountPassword;
    }
}
