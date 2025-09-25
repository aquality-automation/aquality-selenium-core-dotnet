using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using WebElementFactory = Aquality.Selenium.Core.Tests.Applications.Browser.Elements.WebElementFactory;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public abstract class TestWithBrowser
    {
        protected const int RetriesNumber = 10;
        protected static string TestSite => "http://the-internet.herokuapp.com";

        protected ServiceProvider ServiceProvider { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            new CustomStartup().ConfigureServices(services, serviceCollection => AqualityServices.Application);
            ServiceProvider = services.BuildServiceProvider();
            AqualityServices.ServiceProvider = ServiceProvider;
        }

        protected static void GoToUrl(Uri url, IWebDriver driver = null)
        {
            var driverInstance = driver ?? AqualityServices.Application.Driver;
            // temporary workaround to avoid issue described at https://github.com/SeleniumHQ/selenium/issues/12277
            try
            {
                driverInstance.Navigate().GoToUrl(url);
            }
            catch (WebDriverException e) when (driver.Url == url.ToString())
            {
                Logger.Instance.Fatal($"Random error occurred: [{e.Message}], but successfully navigated to URL [{url}]", e);
            }
        }

        [TearDown]
        public void CleanUp()
        {
            if (AqualityServices.IsApplicationStarted)
            {
                AqualityServices.Application.Quit();
            }
        }

        private class CustomStartup : Startup
        {
            public override IServiceCollection ConfigureServices(IServiceCollection services, Func<IServiceProvider, IApplication> applicationProvider, ISettingsFile settings = null)
            {
                base.ConfigureServices(services, applicationProvider, settings);
                services.AddSingleton<IElementFactory, WebElementFactory>();
                return services;
            }
        }
    }
}
