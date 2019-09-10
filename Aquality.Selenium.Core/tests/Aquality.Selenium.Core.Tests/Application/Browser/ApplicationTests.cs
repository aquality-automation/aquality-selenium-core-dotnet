using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Elements.Interfaces;
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
    [Parallelizable(ParallelScope.All)]
    public class ChromeTests
    {
        private ServiceProvider serviceProvider;
        private static readonly TimeSpan littleTimeout = TimeSpan.FromSeconds(1);

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            
            new Startup().ConfigureServices(services, serviceCollection => ApplicationManager.Application);
            serviceProvider = services.BuildServiceProvider();
        }

        [TearDown]
        public void CleanUp()
        {
            if (ApplicationManager.IsStarted)
            {
                ApplicationManager.Application.Quit();
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
            Assert.IsFalse(ApplicationManager.IsStarted);
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

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_WithElementFinder()
        {
            bool elementFinderCondition() => serviceProvider.GetRequiredService<IElementFinder>()
                .FindElements(By.XPath("//*[contains(., 'wikipedia')]"), timeout: littleTimeout).Count > 0;
            Assert.IsFalse(elementFinderCondition());
            Assert.DoesNotThrow(() => serviceProvider.GetService<ConditionalWait>().WaitFor(driver =>
            {
                driver.Navigate().GoToUrl("https://wikipedia.org");
                return elementFinderCondition();
            }));
        }
    }
}
