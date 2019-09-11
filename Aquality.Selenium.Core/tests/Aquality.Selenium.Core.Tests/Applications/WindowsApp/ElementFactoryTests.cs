using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Support.PageObjects;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class ElementFactoryTests : TestWithApplication
    {
        [Test]
        public void Should_WorkWithCalculator_ViaElementFactory()
        {
            var factory = ApplicationManager.ServiceProvider.GetRequiredService<IElementFactory>();
            factory.GetButton(MobileBy.AccessibilityId("num1Button"), "1").Click();
            factory.GetButton(MobileBy.AccessibilityId("plusButton"), "+").Click();
            factory.GetButton(MobileBy.AccessibilityId("num2Button"), "2").Click();
            factory.GetButton(MobileBy.AccessibilityId("equalButton"), "=").Click();
            var result = factory.GetButton(MobileBy.AccessibilityId("CalculatorResults"), "Results bar").Text;
            StringAssert.Contains("3", result);
        }

        [Test]
        public void Should_FindChildElements_ViaElementFactory()
        {
            var factory = ApplicationManager.ServiceProvider.GetRequiredService<IElementFactory>();
            var numberPad = factory.GetButton(MobileBy.AccessibilityId("NumberPad"), "Number pad");
            Assert.IsNotNull(factory.FindChildButton(numberPad, MobileBy.AccessibilityId("num1Button")).GetElement(TimeSpan.Zero));
        }

        [Test]
        public void Should_FindElements_ViaElementFactory()
        {
            var factory = ApplicationManager.ServiceProvider.GetRequiredService<IElementFactory>();
            var numberPad = factory.GetButton(MobileBy.AccessibilityId("NumberPad"), "Number pad");
            Assert.AreEqual(10, factory.FindButtons(new ByChained(numberPad.Locator, By.XPath("//*[contains(@AutomationId,'num')]"))).Count);
        }

        [Test]
        public void Should_ThrowInvalidOperationException_WhenConstructorIsNotDefined_ForFindChildElement()
        {
            var factory = ApplicationManager.ServiceProvider.GetRequiredService<IElementFactory>();
            var numberPad = factory.GetButton(MobileBy.AccessibilityId("NumberPad"), "Number pad");
            Assert.Throws<InvalidOperationException>(() => factory.FindChildElement<Button>(numberPad, MobileBy.AccessibilityId("num1Button")).GetElement());
        }

        [Test]
        public void Should_ThrowInvalidOperationException_WhenConstructorIsNotDefined_ForFindElements()
        {
            var factory = ApplicationManager.ServiceProvider.GetRequiredService<IElementFactory>();
            Assert.Throws<InvalidOperationException>(() => factory.FindElements<Button>(MobileBy.AccessibilityId("num1Button")));
        }
    }
}
