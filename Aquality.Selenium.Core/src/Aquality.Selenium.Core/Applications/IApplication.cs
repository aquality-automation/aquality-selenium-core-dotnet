using System;
using OpenQA.Selenium.Remote;

namespace Aquality.Selenium.Core.Applications
{
    public interface IApplication
    {
        RemoteWebDriver Driver { get; }

        void SetImplicitWaitTimeout(TimeSpan timeout);
    }
}
