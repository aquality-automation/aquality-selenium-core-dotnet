using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using OpenQA.Selenium;

namespace Aquality.Selenium.Core.Tests.Applications.Browser.Elements
{
    public class Label(By locator, string name, ElementState state) : WebElement(locator, name, state)
    {
        protected override string ElementType { get; } = "Label";

        public new IElementCacheHandler Cache => base.Cache;
    }
}
