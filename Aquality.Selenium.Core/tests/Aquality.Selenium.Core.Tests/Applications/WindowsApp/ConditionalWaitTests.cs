using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class ConditionalWaitTests : TestWithApplication
    {
        private static readonly TimeSpan LittleTimeout = TimeSpan.FromSeconds(1);

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_WithDriver()
        {
            Assert.DoesNotThrow(() => ApplicationManager.ServiceProvider.GetService<ConditionalWait>().WaitFor(driver =>
            {
                return driver.FindElements(By.XPath("//*")).Count > 0;
            }));
        }

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_WithElementFinder()
        {
            bool elementFinderCondition() => ApplicationManager.ServiceProvider.GetRequiredService<IElementFinder>()
                .FindElement(MobileBy.AccessibilityId("CalculatorResults"), timeout: LittleTimeout).Text.Contains("3");
            Assert.IsFalse(elementFinderCondition());
            Assert.DoesNotThrow(() => ApplicationManager.ServiceProvider.GetService<ConditionalWait>().WaitFor(driver =>
            {
                driver.FindElement(MobileBy.AccessibilityId("num3Button")).Click();
                return elementFinderCondition();
            }, LittleTimeout));
        }
    }
}
