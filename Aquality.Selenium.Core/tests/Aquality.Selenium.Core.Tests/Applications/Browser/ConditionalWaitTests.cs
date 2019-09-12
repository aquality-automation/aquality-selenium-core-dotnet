using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class ConditionalWaitTests : TestWithBrowser
    {
        private static readonly Uri WikiURL = new Uri("https://wikipedia.org");
        private static readonly TimeSpan LittleTimeout = TimeSpan.FromSeconds(1);

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_WithDriver()
        {
            Assert.DoesNotThrow(() => ServiceProvider.GetService<ConditionalWait>().WaitFor(driver =>
            {
                driver.Navigate().GoToUrl(WikiURL);
                return driver.FindElements(By.XPath("//*")).Count > 0;
            }));
        }

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_WithElementFinder()
        {
            bool elementFinderCondition() => ServiceProvider.GetRequiredService<IElementFinder>()
                .FindElements(By.XPath("//*[contains(., 'wikipedia')]"), timeout: LittleTimeout).Count > 0;
            Assert.IsFalse(elementFinderCondition());
            Assert.DoesNotThrow(() => ServiceProvider.GetService<ConditionalWait>().WaitFor(driver =>
            {
                driver.Navigate().GoToUrl(WikiURL);
                return elementFinderCondition();
            }));
        }
    }
}
