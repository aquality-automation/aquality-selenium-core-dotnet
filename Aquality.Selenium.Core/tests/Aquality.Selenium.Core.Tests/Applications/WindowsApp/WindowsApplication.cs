using System;
using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class WindowsApplication : IApplication
    {
        private TimeSpan implicitWait;
        private readonly Lazy<WebDriver> lazyDriver;

        public WindowsApplication(string application, Uri driverServer, ITimeoutConfiguration timeoutConfiguration, AppiumOptions appiumOptions)
        {
            var options = appiumOptions ?? new AppiumOptions();
            options.App = application;
            options.AutomationName = "windows";
            lazyDriver = new Lazy<WebDriver>(() =>
            {
                var value = new WindowsDriver(driverServer, options, timeoutConfiguration.Command);
                value.Manage().Timeouts().ImplicitWait = timeoutConfiguration.Implicit;
                return value;
            });
        }

        public WebDriver Driver => lazyDriver.Value;

        public bool IsStarted => lazyDriver.IsValueCreated && Driver.SessionId != null;

        public void SetImplicitWaitTimeout(TimeSpan timeout)
        {
            if (timeout != implicitWait)
            {
                Driver.Manage().Timeouts().ImplicitWait = timeout;
                implicitWait = timeout;
            }
        }
    }
}
