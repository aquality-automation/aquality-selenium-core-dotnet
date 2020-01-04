using System;
using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class WindowsApplication : IApplication
    {
        private TimeSpan implicitWait;
        private readonly Lazy<RemoteWebDriver> lazyDriver;

        public WindowsApplication(string application, Uri driverServer, ITimeoutConfiguration timeoutConfiguration, AppiumOptions appiumOptions)
        {
            var options = appiumOptions ?? new AppiumOptions();
            options.AddAdditionalCapability("app", application);
            lazyDriver = new Lazy<RemoteWebDriver>(() =>
            {
                var value = new WindowsDriver<WindowsElement>(driverServer, options, timeoutConfiguration.Command);
                value.Manage().Timeouts().ImplicitWait = timeoutConfiguration.Implicit;
                return value;
            });
        }

        public RemoteWebDriver Driver => lazyDriver.Value;

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
