using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp.Locators
{
    public static class CalculatorWindow
    {
        public static By WindowLocator => By.XPath("//Window");

        public static By OneButton => By.XPath("//*[@Name='1']");

        public static By TwoButton => By.XPath("//*[@Name='2']");

        public static By ThreeButton => By.XPath("//*[@Name='3']");

        public static By PlusButton => By.XPath("//*[@Name='+']");

        public static By EqualsButton => By.XPath("//*[@Name='=']");

        public static By ResultsLabel => MobileBy.AccessibilityId("48");

        public static By EmptyButton => By.XPath("//*[@AutomationId='7']");

        public static By AbsentElement => By.XPath("//*[@Name='Absent element']");
    }
}
