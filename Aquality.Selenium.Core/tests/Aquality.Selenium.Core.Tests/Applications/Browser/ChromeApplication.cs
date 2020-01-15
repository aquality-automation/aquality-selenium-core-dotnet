using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class ChromeApplication : IApplication
    {
        private TimeSpan implicitWait;

        public ChromeApplication(ITimeoutConfiguration timeoutConfiguration)
        {
            Driver = new ChromeDriver();
            implicitWait = timeoutConfiguration.Implicit;
            Driver.Manage().Timeouts().ImplicitWait = implicitWait;
        }

        public RemoteWebDriver Driver { get; }

        public bool IsStarted => Driver.SessionId != null;

        public void SetImplicitWaitTimeout(TimeSpan timeout)
        {
            if (timeout != implicitWait)
            {
                Driver.Manage().Timeouts().ImplicitWait = timeout;
                implicitWait = timeout;
            }
        }

        public void Quit()
        {
            Driver?.Quit();
        }
    }
}
