using GraphSdkDemo.SharePoint.Api;
using GraphSdkDemo.SharePoint.Contracts.Services;
using GraphSdkDemo.SharePoint.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace GraphSdkDemo.SharePoint.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ISettingsProvider, SettingsProvider>();
            builder.Services.AddTransient<IFileService, FileService>();
        }
    }
}