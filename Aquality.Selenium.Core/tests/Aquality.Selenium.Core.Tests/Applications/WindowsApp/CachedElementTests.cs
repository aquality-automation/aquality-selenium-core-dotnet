using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Locators;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class CachedElementTests : TestWithApplication
    {
        private const string ElementCacheVariableName = "elementCache.isEnabled"; 
        
        private static IElementFactory Factory => AqualityServices.ServiceProvider.GetRequiredService<IElementFactory>();

        private static readonly Func<IElementStateProvider, bool>[] StateFunctionsFalseWhenElementStale
            =
            [
                state => state.IsDisplayed,
                state => state.IsExist,                
                state => !state.WaitForNotDisplayed(TimeSpan.Zero),
                state => !state.WaitForNotExist(TimeSpan.Zero),
            ];

        private static readonly Func<IElementStateProvider, bool>[] StateFunctionsTrueWhenElementStaleWhichRetriveSession
            =
            [
                state => state.IsEnabled,
                state => state.IsClickable,
                state => state.WaitForDisplayed(TimeSpan.Zero),
                state => state.WaitForExist(TimeSpan.Zero),
                state => state.WaitForEnabled(TimeSpan.Zero),
                state => !state.WaitForNotEnabled(TimeSpan.Zero),
            ];

        private static readonly Func<IElementStateProvider, bool>[] StateFunctionsThrowNoSuchElementException
            =
            [
                state => state.IsEnabled,
                state => state.WaitForEnabled(TimeSpan.Zero),
                state => !state.WaitForNotEnabled(TimeSpan.Zero),
            ];

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
            Assert.That(result, Contains.Substring("2"));
        }

        [Test]
        public void Should_ReturnSameElement_AfterInteraction()
        {
            const string errorMessage = "Element is not the same after the interaction";
            var oneButton = Factory.GetButton(CalculatorWindow.OneButton, "1");
            var initialElement = oneButton.GetElement();
            oneButton.Click();
            var resultElement = oneButton.GetElement();
            Assert.That(resultElement, Is.EqualTo(initialElement), errorMessage);
        }

        [Test]
        public void Should_ReturnSameElement_AfterGetState()
        {
            const string errorMessage = "Element is not the same after getting state";
            var oneButton = Factory.GetButton(CalculatorWindow.OneButton, "1");
            var initialElement = oneButton.GetElement();
            oneButton.State.WaitForClickable();
            var resultElement = oneButton.GetElement();
            Assert.That(resultElement, Is.EqualTo(initialElement), errorMessage);
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
            Assert.That(resultElement, Is.Not.EqualTo(initialElement), errorMessage);
        }

        [Test]
        public void Should_ThrowNoSuchElementException_ForAbsentElement([ValueSource(nameof(StateFunctionsThrowNoSuchElementException))] Func<IElementStateProvider, bool> stateCondition)
        {
            var button = Factory.GetButton(CalculatorWindow.AbsentElement, "Absent element");
            Assert.Throws<NoSuchElementException>(() => stateCondition.Invoke(button.State));
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

        private static void AssertStateConditionAfterQuit(Func<IElementStateProvider, bool> stateCondition, bool expectedValue, bool shouldAppRestart)
        {
            var oneButton = Factory.GetButton(CalculatorWindow.OneButton, "1");
            oneButton.GetElement();
            AqualityServices.Application.Driver.Quit();
            Assert.That(stateCondition(oneButton.State), Is.EqualTo(expectedValue), "Element state condition is not expected after closing the window");
            Assert.That(AqualityServices.IsApplicationStarted, Is.EqualTo(shouldAppRestart), $"Window was {(shouldAppRestart ? "not " : string.Empty)}reopened when retrived the element state.");
        }

        [TearDown]
        public new void CleanUp()
        {
            Environment.SetEnvironmentVariable(ElementCacheVariableName, null);
            base.CleanUp();
        }
    }
}
