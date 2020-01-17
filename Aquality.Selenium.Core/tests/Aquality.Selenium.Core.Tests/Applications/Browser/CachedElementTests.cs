﻿using Aquality.Selenium.Core.Elements;
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
        private static readonly By StartLoc = By.XPath("//*[@id='start']//button");
        private static readonly By LoadingLoc = By.Id("loading");
        private static readonly Uri DynamicContentUrl = new Uri($"{TestSite}/dynamic_controls");
        private static readonly Uri DynamicLoadingUrl = new Uri($"{TestSite}/dynamic_loading/1");
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
        }

        private void StartLoading()
        {
            AqualityServices.Application.Driver.Navigate().GoToUrl(DynamicLoadingUrl);
            new Label(StartLoc, "start", ElementState.Displayed).Click();
        }

        private void OpenDynamicContent()
        {
            AqualityServices.Application.Driver.Navigate().GoToUrl(DynamicContentUrl);
        }

        private void WaitForLoading(Label loader)
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
            Assert.IsFalse(loader.State.WaitForDisplayed(TimeSpan.Zero), nameof(Should_ReturnFalse_AtWaitForDisplayed_WhenElementIsNotDisplayed));
        }

        [Test]
        public void Should_ReturnTrue_AtWaitForExist_WhenElementIsNotDisplayed()
        {
            var loader = new Label(LoadingLoc, "loader", ElementState.Displayed);
            StartLoading();
            WaitForLoading(loader);
            Assert.IsTrue(loader.State.WaitForExist(TimeSpan.Zero), nameof(Should_ReturnTrue_AtWaitForExist_WhenElementIsNotDisplayed));
        }

        [Test]
        public void Should_BeStale_WhenBecameInvisible()
        {
            StartLoading();
            var loader = new Label(LoadingLoc, "loader", ElementState.Displayed);
            Assume.That(loader.State.WaitForDisplayed(), "Loader should be displayed in the beginning");
            Assert.IsTrue(AqualityServices.ServiceProvider.GetRequiredService<ConditionalWait>().WaitFor(
                () => loader.Cache.IsStale), "Loader should become invisible and be treated as stale");
            Assert.IsFalse(loader.State.IsDisplayed, "Invisible loader should be not displayed");
            Assert.IsFalse(loader.State.IsExist, "Loader that was displayed previously and become invisible should be treated as disappeared");
            Assert.IsTrue(loader.State.WaitForExist(TimeSpan.Zero), "When waiting for existance, we should get an actual element's state");
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
            OpenDynamicContent();
            var testElement = new Label(ContentLoc, "Example", ElementState.ExistsInAnyState);
            testElement.State.WaitForClickable();
            new Label(RemoveButtonLoc, "Remove", ElementState.Displayed).Click();
            AqualityServices.ServiceProvider.GetRequiredService<ConditionalWait>().WaitForTrue(
                () => testElement.Cache.IsStale, message: "Element should be stale when it disappeared.");
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
