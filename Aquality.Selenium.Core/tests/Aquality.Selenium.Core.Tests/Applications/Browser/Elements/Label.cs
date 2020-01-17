using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Tests.Applications.Browser;
using OpenQA.Selenium;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements
{
    public class Label : WebElement
    {
        public Label(By locator, string name, ElementState state) : base(locator, name, state)
        {
        }

        protected override string ElementType { get; } = "Label";

        public new IElementCacheHandler Cache => base.Cache;
    }
}
