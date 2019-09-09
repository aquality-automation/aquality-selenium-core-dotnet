using Aquality.Selenium.Core.Applications;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Aquality.Selenium.Core.Tests.Application
{
    public class ChromeApplication : IApplication
    {
        private static bool isStarted;
        private static object startLock = new object();
        private TimeSpan implicitWait;
        
        private ChromeApplication()
        {
            Driver = new ChromeDriver();
            implicitWait = TimeSpan.Zero;
            Driver.Manage().Timeouts().ImplicitWait = implicitWait;
            isStarted = true;
        }

        public static ChromeApplication Start()
        {
            lock (startLock)
            {
                new DriverManager().SetUpDriver(new ChromeConfig());
                return new ChromeApplication();
            }            
        }

        public static bool IsStarted => isStarted;

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
