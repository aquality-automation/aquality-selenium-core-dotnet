using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Utilities;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Aquality.Selenium.Core.Tests.Application
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class ChromeTests
    {
        private ServiceProvider serviceProvider;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            new Startup().ConfigureServices(services, serviceCollection => ChromeApplication.Start());
            serviceProvider = services.BuildServiceProvider();
        }

        [TearDown]
        public void CleanUp()
        {
            if (ChromeApplication.IsStarted)
            {
                serviceProvider.GetRequiredService<IApplication>().Driver.Quit();
            }
        }

        [Test]
        public void Should_BePossibleTo_UseLocalizationManager()
        {
            Assert.AreEqual("Clicking", serviceProvider.GetService<LocalizationManager>().GetLocalizedMessage("loc.clicking"));
        }

        [Test]
        public void Should_NotStartApplication_ForElementActionsRetrier()
        {
            serviceProvider.GetService<ElementActionRetrier>().DoWithRetry(() => true);
            Assert.IsFalse(ChromeApplication.IsStarted);
        }

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_WithDriver()
        {
            Assert.DoesNotThrow(() => serviceProvider.GetService<ConditionalWait>().WaitFor(driver =>
            {
                driver.Navigate().GoToUrl("https://wikipedia.org");
                return driver.FindElements(By.XPath("//*")).Count > 0;
            }));
        }
    }
}
