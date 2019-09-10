﻿using Aquality.Selenium.Core.Utilities;
using System.Threading;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
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
                var version = EnvironmentConfiguration.GetVariable("webdriverversion") ?? "Latest";
                new DriverManager().SetUpDriver(new ChromeConfig(), version: version);
            }

            return new ChromeApplication();
        }
    }
}
