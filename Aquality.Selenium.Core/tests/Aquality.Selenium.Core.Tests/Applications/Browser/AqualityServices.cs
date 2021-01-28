using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class AqualityServices : AqualityServices<ChromeApplication>
    {
        private static readonly object downloadDriverLock = new object();

        public new static bool IsApplicationStarted => IsApplicationStarted();

        public static ChromeApplication Application => GetApplication(services => StartChrome(services));

        public static IServiceProvider ServiceProvider 
        { 
            get
            {
                return GetServiceProvider(services => Application);
            }
            set
            {
                SetServiceProvider(value);
            }
        }
            

        private static ChromeApplication StartChrome(IServiceProvider services)
        {
            lock (downloadDriverLock)
            {
                new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
            }

            return new ChromeApplication(services.GetRequiredService<ITimeoutConfiguration>());
        }
    }
}
