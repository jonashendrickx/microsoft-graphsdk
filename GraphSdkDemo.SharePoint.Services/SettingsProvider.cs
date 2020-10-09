using System;
using System.Configuration;
using GraphSdkDemo.SharePoint.Contracts.Services;
using Constants = GraphSdkDemo.SharePoint.Contracts.Common.Configuration;

namespace GraphSdkDemo.SharePoint.Services
{
    public class SettingsProvider : ISettingsProvider
    {
        public string GetClientId() => Get(Constants.ClientIdKey);
        public string GetTenantId() => Get(Constants.TenantIdKey);
        public string GetSiteId() => Get(Constants.SiteIdKey);
        public string GetServiceAccountUsername() => Get(Constants.ServiceAccountUsernameKey);
        public string GetServiceAccountPassword() => Get(Constants.ServiceAccountPasswordKey);

        private string Get(string key)
        {
            return Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process) ?? throw new ConfigurationErrorsException($"Setting '{key}' is not configured or settings file is malformed.");
        }
    }
}
