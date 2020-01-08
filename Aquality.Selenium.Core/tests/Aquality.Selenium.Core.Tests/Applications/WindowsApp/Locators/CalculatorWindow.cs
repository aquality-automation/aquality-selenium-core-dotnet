using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp.Locators
{
    public static class CalculatorWindow
    {
        public static By WindowLocator => By.TagName("Window");

        public static By OneButton => By.Name("1");

        public static By TwoButton => By.Name("2");

        public static By ThreeButton => By.Name("3");

        public static By PlusButton => By.Name("+");

        public static By EqualsButton => By.Name("=");

        public static By ResultsLabel => MobileBy.AccessibilityId("48");

        public static By EmptyButton => By.XPath("//*[@AutomationId='7']");
    }
}
