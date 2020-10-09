using System;
using System.IO;
using GraphSdkDemo.SharePoint.Api;
using GraphSdkDemo.SharePoint.Contracts.Services;
using GraphSdkDemo.Tests.Common.Dependencies.Loggers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GraphSdkDemo.SharePoint.Api.IntegrationTests.Dependencies
{
    public class TestHost
    {
        public TestHost()
        {
            var startup = new TestStartup();
            var host = new HostBuilder()
                .ConfigureWebJobs(startup.Configure)
                .ConfigureServices(ReplaceTestOverrides)
                .Build();

            ServiceProvider = host.Services;
        }

        public IServiceProvider ServiceProvider { get; }

        private void ReplaceTestOverrides(IServiceCollection services)
        {
            services.AddScoped<ILogger, ListLogger>();
            services.Replace(ServiceDescriptor.Singleton(typeof(ISettingsProvider), typeof(SettingsProviderMock)));
        }

        private class TestStartup : Startup
        {
            public override void Configure(IFunctionsHostBuilder builder)
            {
                SetExecutionContextOptions(builder);
                base.Configure(builder);
            }

            private static void SetExecutionContextOptions(IFunctionsHostBuilder builder)
            {
                builder.Services.Configure<ExecutionContextOptions>(o => o.AppDirectory = Directory.GetCurrentDirectory());
            }
        }
    }
}