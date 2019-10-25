using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Service;
using System;
using System.IO;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class ApplicationManager : ApplicationManager<WindowsApplication>
    {
        private const string SupportedApplication = "./Resources/WindowsApp/Day Maxi Calc.exe";
        private const string DefaultDriverServer = "http://127.0.0.1:4723/";
        
        public static WindowsApplication Application => GetApplication(service => StartApplication(service));

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
            return new WindowsApplication(Path.GetFullPath(SupportedApplication), driverServer, services.GetRequiredService<ITimeoutConfiguration>(), options);
        }
    }
}
