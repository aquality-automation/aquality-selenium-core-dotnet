using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class CachedElementTests : TestWithBrowser
    {
        private static readonly By RemoveButtonLoc = By.XPath("//button[.='Remove']");
        private static readonly By ContentLoc = By.Id("checkbox");
        private static readonly Uri DynamicContentUrl = new Uri("http://the-internet.herokuapp.com/dynamic_controls");
        private const string ElementCacheVariableName = "elementCache.isEnabled"; 
        
        private static readonly Func<IElementStateProvider, bool>[] StateFunctionsFalseWhenElementStale
            = new Func<IElementStateProvider, bool>[]
            {
                state => state.IsDisplayed,
                state => state.IsExist,            
                state => !state.WaitForNotDisplayed(TimeSpan.Zero),
                state => !state.WaitForNotExist(TimeSpan.Zero),
            };

        private static readonly Func<IElementStateProvider, bool>[] StateFunctionsTrueWhenElementStaleWhichRetriveElement
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
        public new void SetUp()
        {
            Environment.SetEnvironmentVariable(ElementCacheVariableName, true.ToString());
            AqualityServices.Application.Driver.Navigate().GoToUrl(DynamicContentUrl);
        }

        [Test]
        public void Should_RefreshElement_WhenItIsStale()
        {
            var example = new Label(ContentLoc, "Example", ElementState.Displayed);
            example.GetElement();
            var exToString = example.GetElement().ToString();
            AqualityServices.Application.Driver.Navigate().Refresh();
            var newToString = example.GetElement().ToString();
            Assert.AreNotEqual(exToString, newToString);

        }
        
        [Test]
        public void Should_ReturnCorrectState_False_WhenWindowIsRefreshed([ValueSource(nameof(StateFunctionsFalseWhenElementStale))] Func<IElementStateProvider, bool> stateCondition)
        {
            AssertStateConditionAfterRefresh(stateCondition, expectedValue: false);
        }

        [Test]
        public void Should_ReturnCorrectState_True_WhenWindowIsRefreshed([ValueSource(nameof(StateFunctionsTrueWhenElementStaleWhichRetriveElement))] Func<IElementStateProvider, bool> stateCondition)
        {
            AssertStateConditionAfterRefresh(stateCondition, expectedValue: true);
        }

        private void AssertStateConditionAfterRefresh(Func<IElementStateProvider, bool> stateCondition, bool expectedValue)
        {
            var testElement = new Label(ContentLoc, "Example", ElementState.ExistsInAnyState);
            testElement.State.WaitForClickable();
            new Label(RemoveButtonLoc, "Remove", ElementState.Displayed).Click();
            AqualityServices.ServiceProvider.GetRequiredService<ConditionalWait>().WaitFor(driver =>
            {
                return testElement.Cache.IsStale;
            }, message: "Element should be stale when it disappeared.");
            AqualityServices.Application.Driver.Navigate().Refresh();
            Assert.IsTrue(testElement.Cache.IsStale, "Element should remain stale after the page refresh.");
            Assert.AreEqual(expectedValue, stateCondition(testElement.State), 
                "Element state condition is not expected after refreshing the window");            
        }

        [TearDown]
        public new void CleanUp()
        {
            Environment.SetEnvironmentVariable(ElementCacheVariableName, null);
            base.CleanUp();
        }
    }
}
