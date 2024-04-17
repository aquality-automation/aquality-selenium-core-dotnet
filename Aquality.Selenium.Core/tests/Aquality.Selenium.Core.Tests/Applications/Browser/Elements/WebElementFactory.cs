using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Utilities;
using Aquality.Selenium.Core.Waitings;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace Aquality.Selenium.Core.Tests.Applications.Browser.Elements
{
    internal class WebElementFactory(IConditionalWait conditionalWait, IElementFinder elementFinder, ILocalizationManager localizationManager) : ElementFactory(conditionalWait, elementFinder, localizationManager)
    {
        protected override By GenerateXpathLocator(By baseLocator, IWebElement webElement, int elementIndex)
        {
            return baseLocator.ToString().StartsWith("By.XPath")
                ? base.GenerateXpathLocator(baseLocator, webElement, elementIndex)
                : By.XPath(AqualityServices.Application.Driver.ExecuteJavaScript<string>(
                    FileReader.GetTextFromEmbeddedResource("Resources.GetElementXPath.js"), webElement));
        }
    }
}
