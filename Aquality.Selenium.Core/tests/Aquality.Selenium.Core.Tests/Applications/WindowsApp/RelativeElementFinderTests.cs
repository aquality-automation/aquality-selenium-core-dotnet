using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Locators;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class RelativeElementFinderTests : TestWithApplication
    {
        private IElementFinder ElementFinder => new RelativeElementFinder(
            AqualityServices.ServiceProvider.GetRequiredService<ILocalizedLogger>(),
            AqualityServices.ServiceProvider.GetRequiredService<ConditionalWait>(),
            () => AqualityServices.Application.Driver.FindElement(CalculatorWindow.WindowLocator));
        
        [Test]
        public void Should_FindChildElements_ViaRelativeElementFinder()
        {
            Assert.IsNotNull(ElementFinder.FindElement(CalculatorWindow.OneButton));
        }
    }
}
