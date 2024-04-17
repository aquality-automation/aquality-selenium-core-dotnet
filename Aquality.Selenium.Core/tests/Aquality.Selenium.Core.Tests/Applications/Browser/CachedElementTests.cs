using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Tests.Applications.Browser.Elements;
using Aquality.Selenium.Core.Utilities;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    [Parallelizable(ParallelScope.Children)]
    public class CachedElementTests : TestWithBrowser
    {
        private static readonly By ContentLoc = By.Id("checkbox");
        private static readonly By StartLoc = By.XPath("//*[@id='start']//button");
        private static readonly By LoadingLoc = By.Id("loading");
        private static readonly Uri DynamicContentUrl = new($"{TestSite}/dynamic_controls");
        private static readonly Uri DynamicLoadingUrl = new($"{TestSite}/dynamic_loading/1");
        
        private const string ElementCacheVariableName = "elementCache.isEnabled"; 
        
        private static readonly Func<IElementStateProvider, bool>[] StateFunctionsFalseWhenElementStale
            =
            [
                state => state.IsDisplayed,
                state => state.IsExist,            
                state => !state.WaitForNotDisplayed(TimeSpan.Zero),
                state => !state.WaitForNotExist(TimeSpan.Zero),
            ];

        private static readonly Func<IElementStateProvider, bool>[] StateFunctionsTrueWhenElementStaleWhichRetriveElement
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

        private static IConditionalWait ConditionalWait => AqualityServices.ServiceProvider.GetRequiredService<IConditionalWait>();

        [SetUp]
        public new void SetUp()
        {
            Environment.SetEnvironmentVariable(ElementCacheVariableName, true.ToString());
        }

        private static void StartLoading()
        {
            GoToUrl(DynamicLoadingUrl);
            new Label(StartLoc, "start", ElementState.Displayed).Click();
        }

        private static void OpenDynamicContent()
        {
            GoToUrl(DynamicContentUrl);
        }

        private static void WaitForLoading(Label loader)
        {
            Assume.That(loader.State.WaitForDisplayed(), "Loader should be displayed in the beginning");
            Assume.That(loader.State.WaitForNotDisplayed(), "Loader should not be displayed in the end");
        }

        [Test]
        public void Should_ReturnFalse_AtWaitForDisplayed_WhenElementIsNotDisplayed()
        {
            var loader = new Label(LoadingLoc, "loader", ElementState.Displayed);
            StartLoading();
            WaitForLoading(loader);
            Assert.That(loader.State.WaitForDisplayed(TimeSpan.Zero), Is.False, nameof(Should_ReturnFalse_AtWaitForDisplayed_WhenElementIsNotDisplayed));
        }

        [Test]
        public void Should_ReturnTrue_AtWaitForExist_WhenElementIsNotDisplayed()
        {
            var loader = new Label(LoadingLoc, "loader", ElementState.Displayed);
            StartLoading();
            WaitForLoading(loader);
            Assert.That(loader.State.WaitForExist(TimeSpan.Zero), nameof(Should_ReturnTrue_AtWaitForExist_WhenElementIsNotDisplayed));
        }

        [Test]
        public void Should_BeStale_WhenBecameInvisible()
        {
            StartLoading();
            var loader = new Label(LoadingLoc, "loader", ElementState.Displayed);
            Assume.That(loader.State.WaitForDisplayed(), "Loader should be displayed in the beginning");
            Assert.That(ConditionalWait.WaitFor(() => loader.Cache.IsStale), "Loader should become invisible and be treated as stale");
            Assert.That(loader.State.IsDisplayed, Is.False, "Invisible loader should be not displayed");
            Assert.That(loader.State.IsExist, Is.False, "Loader that was displayed previously and become invisible should be treated as disappeared");
            Assert.That(loader.State.WaitForExist(TimeSpan.Zero), "When waiting for existance, we should get an actual element's state");
        }

        [Test]
        public void Should_RefreshElement_WhenItIsStale()
        {
            OpenDynamicContent();
            var example = new Label(ContentLoc, "Example", ElementState.Displayed);
            example.GetElement();
            var exToString = example.GetElement().ToString();
            AqualityServices.Application.Driver.Navigate().Refresh();
            var newToString = example.GetElement().ToString();
            Assert.That(newToString, Is.Not.EqualTo(exToString));
        }
        
        [Test]
        public void Should_ThrowNoSuchElementException_ForAbsentElement([ValueSource(nameof(StateFunctionsThrowNoSuchElementException))] Func<IElementStateProvider, bool> stateCondition)
        { 
            var label = new Label(By.Name("Absent element"), "Absent element", ElementState.Displayed);
            Assert.Throws<NoSuchElementException>(() => stateCondition.Invoke(label.State));
        }
        
        [Test]
        [Ignore("Tests should be updated: find out more stable example")]
        public void Should_ReturnCorrectState_False_WhenWindowIsReopened([ValueSource(nameof(StateFunctionsFalseWhenElementStale))] Func<IElementStateProvider, bool> stateCondition)
        {
            AssertStateConditionAfterReopen(stateCondition, expectedValue: false);
        }

        [Test]
        [Ignore("Tests should be updated: find out more stable example")]
        public void Should_ReturnCorrectState_True_WhenWindowIsReopened([ValueSource(nameof(StateFunctionsTrueWhenElementStaleWhichRetriveElement))] Func<IElementStateProvider, bool> stateCondition)
        {
            AssertStateConditionAfterReopen(stateCondition, expectedValue: true);
        }

        private static void AssertStateConditionAfterReopen(Func<IElementStateProvider, bool> stateCondition, bool expectedValue)
        {
            Label testElement = null;
            AqualityServices.ServiceProvider.GetRequiredService<IActionRetrier>().DoWithRetry(() =>
            {
                if (AqualityServices.IsApplicationStarted)
                {
                    AqualityServices.Application.Quit();
                }
                ConditionalWait.WaitForTrue(() =>
                {
                    OpenDynamicContent();
                    testElement = new Label(ContentLoc, "Example", ElementState.ExistsInAnyState);
                    testElement.State.WaitForClickable();
                    StartLoading();
                    return testElement.Cache.IsStale;
                }, message: "Element should be stale after page is closed.");
            },
            new[] { typeof(TimeoutException) });
            
            Assume.That(testElement, Is.Not.Null);
            OpenDynamicContent();
            Assert.That(stateCondition(testElement.State), Is.EqualTo(expectedValue),
                "Element state condition is not expected after reopening the window");            
        }

        [TearDown]
        public new void CleanUp()
        {
            Environment.SetEnvironmentVariable(ElementCacheVariableName, null);
            base.CleanUp();
        }
    }
}
