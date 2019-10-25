using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class ApplicationManager : ApplicationManager<ChromeApplication>
    {
        private static readonly object downloadDriverLock = new object();

        public static ChromeApplication Application => GetApplication(services => StartChrome(services));
        
        private static ChromeApplication StartChrome(IServiceProvider services)
        {
            lock (downloadDriverLock)
            {
                var version = EnvironmentConfiguration.GetVariable("webdriverversion") ?? "77.0.3865.40";
                new DriverManager().SetUpDriver(new ChromeConfig(), version: version);
            }

            return new ChromeApplication(services.GetRequiredService<ITimeoutConfiguration>());
        }
    }
}
