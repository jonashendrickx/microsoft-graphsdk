namespace GraphSdkDemo.SharePoint.Contracts.Services
{
    public interface ISettingsProvider
    {
        string GetClientId();
        string GetTenantId();
        string GetSiteId();
        string GetServiceAccountUsername();
        string GetServiceAccountPassword();
    }
}
