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
                .FindElement(CalculatorWindow.ResultsLabel, timeout: LittleTimeout).Text.Contains("3");
            Assert.IsFalse(elementFinderCondition());
            Assert.DoesNotThrow(() => ApplicationManager.ServiceProvider.GetService<ConditionalWait>().WaitFor(driver =>
            {
                driver.FindElement(CalculatorWindow.TwoButton).Click();
                driver.FindElement(CalculatorWindow.PlusButton).Click();
                driver.FindElement(CalculatorWindow.OneButton).Click();
                driver.FindElement(CalculatorWindow.EqualsButton).Click();
                var first = driver.FindElement(By.TagName("Window")).Text;
                var pageSource = driver.PageSource;
                return elementFinderCondition();
            }, LittleTimeout));
        }
    }
}
