using System.Threading;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Aquality.Selenium.Core.Tests.Application
{
    public static class ApplicationManager
    {
        private static readonly object downloadDriverLock = new object();
        private static readonly ThreadLocal<ChromeApplication> BrowserContainer = new ThreadLocal<ChromeApplication>();

        public static bool IsStarted => BrowserContainer.IsValueCreated && BrowserContainer.Value.Driver.SessionId != null;

        public static ChromeApplication Application
        {
            get
            {
                if (!IsStarted)
                {
                    BrowserContainer.Value = Start();
                }
                return BrowserContainer.Value;
            }
            set
            {
                BrowserContainer.Value = value;
            }
        }


        private static ChromeApplication Start()
        {
            lock (downloadDriverLock)
            {
                new DriverManager().SetUpDriver(new ChromeConfig());
            }

            return new ChromeApplication();
        }
    }
}
