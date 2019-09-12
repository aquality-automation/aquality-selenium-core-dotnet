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

        public WindowsApplication(string application, Uri driverServer, ITimeoutConfiguration timeoutConfiguration, AppiumOptions appiumOptions)
        {
            var options = appiumOptions ?? new AppiumOptions();
            options.AddAdditionalCapability("app", application);
            Driver = new WindowsDriver<WindowsElement>(driverServer, options, timeoutConfiguration.Command);
            Driver.Manage().Timeouts().ImplicitWait = timeoutConfiguration.Implicit;
        }

        public RemoteWebDriver Driver { get; }

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
