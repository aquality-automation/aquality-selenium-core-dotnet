using Aquality.Selenium.Core.Applications;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class ChromeApplication : IApplication
    {
        private TimeSpan implicitWait;

        public ChromeApplication()
        {
            Driver = new ChromeDriver();
            implicitWait = TimeSpan.Zero;
            Driver.Manage().Timeouts().ImplicitWait = implicitWait;
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

        public void Quit()
        {
            Driver?.Quit();
        }
    }
}
