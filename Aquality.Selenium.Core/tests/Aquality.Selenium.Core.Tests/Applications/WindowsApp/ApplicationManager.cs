using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Service;
using System;
using System.Threading;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public static class ApplicationManager
    {
        private const string SupportedApplication = "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App";
        private const string DefaultDriverServer = "http://127.0.0.1:4723/";
        private static readonly ThreadLocal<WindowsApplication> AppContainer = new ThreadLocal<WindowsApplication>();
        private static readonly ThreadLocal<IServiceProvider> ServiceProviderContainer = new ThreadLocal<IServiceProvider>();

        public static bool IsStarted => AppContainer.IsValueCreated && AppContainer.Value.Driver.SessionId != null;

        public static WindowsApplication Application
        {
            get
            {
                if (!IsStarted)
                {
                    AppContainer.Value = StartApplication(ServiceProvider);
                }
                return AppContainer.Value;
            }
        }

        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (!ServiceProviderContainer.IsValueCreated)
                {
                    var services = new ServiceCollection();
                    new Startup().ConfigureServices(services, serviceCollection => Application);
                    ServiceProviderContainer.Value = services.BuildServiceProvider();
                }
                return ServiceProviderContainer.Value;
            }
        }

        private static WindowsApplication StartApplication(IServiceProvider services)
        {
            var isRemoteFromEnv = EnvironmentConfiguration.GetVariable("isRemote");
            bool.TryParse(isRemoteFromEnv, out var isRemote);
            Uri driverServer;
            AppiumOptions options = new AppiumOptions();
            if (!isRemote)
            {
                var driverService = AppiumLocalService.BuildDefaultService();
                driverService.Start();
                driverServer = driverService.ServiceUrl;
                options.AddAdditionalCapability("platformVersion", "10");
                options.AddAdditionalCapability("platformName", "Windows");
                options.AddAdditionalCapability("deviceName", "WindowsPC");
            }
            else
            {
                driverServer = new Uri(EnvironmentConfiguration.GetVariable("driverServer") ?? DefaultDriverServer);
            }
            return new WindowsApplication(SupportedApplication, driverServer, services.GetRequiredService<ITimeoutConfiguration>(), options);
        }
    }
}
