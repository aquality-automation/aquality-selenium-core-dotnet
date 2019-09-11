using Aquality.Selenium.Core.Elements;
using OpenQA.Selenium;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements
{
    public class Button : WindowElement
    {
        public Button(By locator, string name) : base(locator, name)
        {
        }

        protected override string ElementType => "Button";
    }
}
