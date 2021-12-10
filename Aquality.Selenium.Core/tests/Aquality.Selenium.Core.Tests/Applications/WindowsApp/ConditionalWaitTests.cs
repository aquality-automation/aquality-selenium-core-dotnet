using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Locators;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class ConditionalWaitTests : TestWithApplication
    {
        private static readonly TimeSpan LittleTimeout = TimeSpan.FromSeconds(1);
        private IConditionalWait ConditionalWait => AqualityServices.ServiceProvider.GetRequiredService<IConditionalWait>();

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_WithDriver()
        {
            Assert.DoesNotThrow(() => ConditionalWait.WaitFor(driver =>
            {
                return driver.FindElements(By.XPath("//*")).Count > 0;
            }));
        }

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_WithElementFinder()
        {
            bool elementFinderCondition() => AqualityServices.ServiceProvider.GetRequiredService<IElementFinder>()
                .FindElement(CalculatorWindow.ResultsLabel, timeout: LittleTimeout).Text.Contains("3");
            Assert.IsFalse(elementFinderCondition());
            Assert.DoesNotThrow(() => ConditionalWait.WaitFor(driver =>
            {
                driver.FindElement(CalculatorWindow.TwoButton).Click();
                driver.FindElement(CalculatorWindow.PlusButton).Click();
                driver.FindElement(CalculatorWindow.OneButton).Click();
                driver.FindElement(CalculatorWindow.EqualsButton).Click();
                var first = driver.FindElement(CalculatorWindow.WindowLocator).Text;
                var pageSource = driver.PageSource;
                return elementFinderCondition();
            }, LittleTimeout));
        }
    }
}
