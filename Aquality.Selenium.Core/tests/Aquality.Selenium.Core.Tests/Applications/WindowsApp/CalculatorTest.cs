using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Locators;
using NUnit.Framework;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class CalculatorTest : TestWithApplication
    {
        [Test]
        public void Should_WorkWithCalculator()
        {
            ApplicationManager.Application.Driver.FindElement(CalculatorWindow.OneButton).Click();
            ApplicationManager.Application.Driver.FindElement(CalculatorWindow.PlusButton).Click();
            ApplicationManager.Application.Driver.FindElement(CalculatorWindow.TwoButton).Click();
            ApplicationManager.Application.Driver.FindElement(CalculatorWindow.EqualsButton).Click();
            var result = ApplicationManager.Application.Driver.FindElement(CalculatorWindow.ResultsLabel).Text;
            StringAssert.Contains("3", result);
        }
    }
}
