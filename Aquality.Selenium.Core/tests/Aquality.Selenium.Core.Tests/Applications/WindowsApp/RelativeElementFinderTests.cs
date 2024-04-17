using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Locators;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class RelativeElementFinderTests : TestWithApplication
    {
        private static IConditionalWait ConditionalWait => AqualityServices.ServiceProvider.GetRequiredService<IConditionalWait>();

        private static IElementFinder ElementFinder => new RelativeElementFinder(
            AqualityServices.ServiceProvider.GetRequiredService<ILocalizedLogger>(),
            ConditionalWait,
            () => AqualityServices.Application.Driver.FindElement(CalculatorWindow.WindowLocator));

        private static IElementStateProvider GetElementStateProvider(By locator) => new ElementStateProvider(
            locator,
            ConditionalWait,
            ElementFinder,
            (messageKey, stateKey) => AqualityServices.ServiceProvider.GetRequiredService<ILocalizedLogger>().Debug(messageKey, args: stateKey));
        
        [Test]
        public void Should_FindChildElements_ViaRelativeElementFinder()
        {
            Assert.That(ElementFinder.FindElement(CalculatorWindow.OneButton), Is.Not.Null);
        }

        [Test]
        public void Should_ThrowException_IfWaitForClickableEnded()
        {
            var emptyButtonState = GetElementStateProvider(CalculatorWindow.EmptyButton);
            Assert.Throws<WebDriverTimeoutException>(() => emptyButtonState.WaitForClickable(TimeSpan.Zero));
        }
    }
}
