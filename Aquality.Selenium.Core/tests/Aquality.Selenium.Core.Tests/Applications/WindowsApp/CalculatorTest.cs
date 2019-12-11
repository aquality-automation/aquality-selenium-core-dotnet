using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Locators;
using NUnit.Framework;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class CalculatorTest : TestWithApplication
    {
        [Test]
        public void Should_WorkWithCalculator()
        {
            AqualityServices.Application.Driver.FindElement(CalculatorWindow.OneButton).Click();
            AqualityServices.Application.Driver.FindElement(CalculatorWindow.PlusButton).Click();
            AqualityServices.Application.Driver.FindElement(CalculatorWindow.TwoButton).Click();
            AqualityServices.Application.Driver.FindElement(CalculatorWindow.EqualsButton).Click();
            var result = AqualityServices.Application.Driver.FindElement(CalculatorWindow.ResultsLabel).Text;
            StringAssert.Contains("3", result);
        }
    }
}
