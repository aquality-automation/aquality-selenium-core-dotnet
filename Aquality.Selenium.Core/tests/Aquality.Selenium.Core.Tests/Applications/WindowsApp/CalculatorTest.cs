using NUnit.Framework;
using OpenQA.Selenium.Appium;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class CalculatorTest : TestWithApplication
    {
        [Test]
        public void Should_WorkWithCalculator()
        {
            ApplicationManager.Application.Driver.FindElement(MobileBy.AccessibilityId("num1Button")).Click();
            ApplicationManager.Application.Driver.FindElement(MobileBy.AccessibilityId("plusButton")).Click();
            ApplicationManager.Application.Driver.FindElement(MobileBy.AccessibilityId("num2Button")).Click();
            ApplicationManager.Application.Driver.FindElement(MobileBy.AccessibilityId("equalButton")).Click();
            var result = ApplicationManager.Application.Driver.FindElement(MobileBy.AccessibilityId("CalculatorResults")).Text;
            StringAssert.Contains("3", result);
        }
    }
}
