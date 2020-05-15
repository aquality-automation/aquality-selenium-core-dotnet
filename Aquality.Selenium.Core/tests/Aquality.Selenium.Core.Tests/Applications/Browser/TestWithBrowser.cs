using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Tests.Applications.Browser.Elements;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public abstract class TestWithBrowser
    {
        protected static string TestSite => "http://the-internet.herokuapp.com";

        protected ServiceProvider ServiceProvider { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            new CustomStartup().ConfigureServices(services, serviceCollection => AqualityServices.Application);
            ServiceProvider = services.BuildServiceProvider();
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
