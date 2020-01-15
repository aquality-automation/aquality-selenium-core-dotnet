using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Locators;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class CachedElementTests : TestWithApplication
    {
        private const string ElementCacheVariableName = "elementCache.enable"; 
        
        private IElementFactory Factory => AqualityServices.ServiceProvider.GetRequiredService<IElementFactory>();

        private static readonly Func<IElementStateProvider, bool>[] StateFunctionsFalseWhenElementStale
            = new Func<IElementStateProvider, bool>[]
            {
                state => state.IsDisplayed,
                state => state.IsExist,                
                state => !state.WaitForNotDisplayed(TimeSpan.Zero),
                state => !state.WaitForNotExist(TimeSpan.Zero),
            };

        private static readonly Func<IElementStateProvider, bool>[] StateFunctionsTrueWhenElementStaleWhichRetriveSession
            = new Func<IElementStateProvider, bool>[]
            {
                state => state.IsEnabled,
                state => state.IsClickable,
                state => state.WaitForDisplayed(TimeSpan.Zero),
                state => state.WaitForExist(TimeSpan.Zero),
                state => state.WaitForEnabled(TimeSpan.Zero),
                state => !state.WaitForNotEnabled(TimeSpan.Zero),
            };

        [SetUp]
        public void SetUp()
        {
            Environment.SetEnvironmentVariable(ElementCacheVariableName, true.ToString());
        }

        [Test]
        public void Should_WorkWithCalculator_WithCachedElement()
        {
            var oneButton = Factory.GetButton(CalculatorWindow.OneButton, "1");            
            oneButton.Click();
            Factory.GetButton(CalculatorWindow.PlusButton, "+").Click();
            oneButton.Click();
            Factory.GetButton(CalculatorWindow.EqualsButton, "=").Click();
            var result = Factory.GetButton(CalculatorWindow.ResultsLabel, "Results bar").Text;
            StringAssert.Contains("2", result);
        }

        [Test]
        public void Should_ReturnSameElement_AfterInteraction()
        {
            const string errorMessage = "Element is not the same after the interaction";
            var oneButton = Factory.GetButton(CalculatorWindow.OneButton, "1");
            var initialElement = oneButton.GetElement();
            oneButton.Click();
            var resultElement = oneButton.GetElement();
            Assert.AreEqual(initialElement, resultElement, errorMessage);
        }

        [Test]
        public void Should_ReturnSameElement_AfterGetState()
        {
            const string errorMessage = "Element is not the same after getting state";
            var oneButton = Factory.GetButton(CalculatorWindow.OneButton, "1");
            var initialElement = oneButton.GetElement();
            oneButton.State.WaitForClickable();
            var resultElement = oneButton.GetElement();
            Assert.AreEqual(initialElement, resultElement, errorMessage);
        }

        [Test]
        public void Should_ReturnNewElement_WhenWindowIsReopened()
        {
            const string errorMessage = "Element is still the same after reopening the window";
            var oneButton = Factory.GetButton(CalculatorWindow.OneButton, "1");
            var initialElement = oneButton.GetElement().ToString();
            AqualityServices.Application.Driver.Quit();
            oneButton.State.WaitForClickable();
            var resultElement = oneButton.GetElement().ToString();
            Assert.AreNotEqual(initialElement, resultElement, errorMessage);
        }

        [Test]
        public void Should_ReturnCorrectState_WhenWindowIsClosed([ValueSource(nameof(StateFunctionsFalseWhenElementStale))] Func<IElementStateProvider, bool> stateCondition)
        {
            AssertStateConditionAfterQuit(stateCondition, expectedValue: false, shouldAppRestart: false);
        }

        [Test]
        public void Should_ReturnCorrectState_AndReopenApplication_WhenWindowIsClosed([ValueSource(nameof(StateFunctionsTrueWhenElementStaleWhichRetriveSession))] Func<IElementStateProvider, bool> stateCondition)
        {
            AssertStateConditionAfterQuit(stateCondition, expectedValue: true, shouldAppRestart: true);
        }

        private void AssertStateConditionAfterQuit(Func<IElementStateProvider, bool> stateCondition, bool expectedValue, bool shouldAppRestart)
        {
            var oneButton = Factory.GetButton(CalculatorWindow.OneButton, "1");
            oneButton.GetElement();
            AqualityServices.Application.Driver.Quit();
            Assert.AreEqual(expectedValue, stateCondition(oneButton.State), "Element state condition is not expected after closing the window");
            Assert.AreEqual(shouldAppRestart, AqualityServices.IsApplicationStarted, $"Window was {(shouldAppRestart ? "not " : string.Empty)}reopened when retrived the element state.");
        }

        [TearDown]
        public new void CleanUp()
        {
            Environment.SetEnvironmentVariable(ElementCacheVariableName, null);
            base.CleanUp();
        }
    }
}
