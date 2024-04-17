using OpenQA.Selenium;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements
{
    public class Button(By locator, string name) : WindowElement(locator, name)
    {
        protected override string ElementType => "Button";
    }
}
