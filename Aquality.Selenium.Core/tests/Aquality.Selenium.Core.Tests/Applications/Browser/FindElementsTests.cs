﻿using NUnit.Framework;
using System;
using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class FindElementsTests : TestWithBrowser
    {
        private static readonly By HiddenElementsLoc = By.XPath("//h5");
        private static readonly By DisplayedElementsLoc = By.XPath("//img[@alt='User Avatar']");
        private static readonly By NotExistElementLoc = By.XPath("//div[@class='testtest']");
        private static readonly By ContentLoc = By.Id("content");
        private static readonly Uri HoversURL = new Uri("http://the-internet.herokuapp.com/hovers");
        private IElementFactory elementFactory;

        [SetUp]
        public new void SetUp()
        {
            elementFactory = ServiceProvider.GetRequiredService<IElementFactory>();
            ApplicationManager.Application.Driver.Navigate().GoToUrl(HoversURL);
        }

        [TestCase(ElementsCount.MoreThenZero, ElementState.Displayed, 3)]
        [TestCase(ElementsCount.MoreThenZero, ElementState.ExistsInAnyState, 3)]
        [TestCase(ElementsCount.Any, ElementState.Displayed, 3)]
        [TestCase(ElementsCount.Any, ElementState.ExistsInAnyState, 3)]
        public void Should_BePossibleTo_FindElements_ForDisplayedElements(ElementsCount count, ElementState state, int expectedCount)
        {
            var elementsCount = elementFactory.FindElements<Label>(DisplayedElementsLoc, expectedCount: count, state: state).Count;
            Assert.AreEqual(expectedCount, elementsCount, $"Elements count for displayed elements should be {expectedCount}");
        }

        [TestCase(ElementsCount.Zero, ElementState.Displayed, 0)]
        [TestCase(ElementsCount.MoreThenZero, ElementState.ExistsInAnyState, 3)]
        [TestCase(ElementsCount.Any, ElementState.Displayed, 0)]
        [TestCase(ElementsCount.Any, ElementState.ExistsInAnyState, 3)]
        public void Should_BePossibleTo_FindElements_ForHiddenElements(ElementsCount count, ElementState state, int expectedCount)
        {
            var elementsCount = elementFactory.FindElements<Label>(HiddenElementsLoc, expectedCount: count, state: state).Count;
            Assert.AreEqual(expectedCount, elementsCount, $"Elements count for hidden elements should be {expectedCount}");
        }

        [TestCase(ElementsCount.Zero, ElementState.Displayed, 0)]
        [TestCase(ElementsCount.Zero, ElementState.ExistsInAnyState, 0)]
        [TestCase(ElementsCount.Any, ElementState.Displayed, 0)]
        [TestCase(ElementsCount.Any, ElementState.ExistsInAnyState, 0)]
        public void Should_BePossibleTo_FindElements_ForNotExistsElements(ElementsCount count, ElementState state, int expectedCount)
        {
            var elementsCount = elementFactory.FindElements<Label>(NotExistElementLoc, expectedCount: count, state: state).Count;
            Assert.AreEqual(expectedCount, elementsCount, $"Elements count for not existing elements should be {expectedCount}");
        }

        [TestCase(ElementsCount.Zero, ElementState.Displayed)]
        [TestCase(ElementsCount.Zero, ElementState.ExistsInAnyState)]
        public void Should_BeImpossibleTo_FindDisplayedElements_WithWrongArguments(ElementsCount count, ElementState state)
        {
            Assert.Throws<WebDriverTimeoutException>(
                () => elementFactory.FindElements<Label>(DisplayedElementsLoc, expectedCount: count, state: state),
                $"Tried to find elements with expected count '{count}' and state '{state}'");
        }

        [TestCase(ElementsCount.MoreThenZero, ElementState.Displayed)]
        [TestCase(ElementsCount.Zero, ElementState.ExistsInAnyState)]
        public void Should_BeImpossibleTo_FindHiddenElements_WithWrongArguments(ElementsCount count, ElementState state)
        {
            var label = new Label(ContentLoc, "Hover form", ElementState.Displayed);
            label.State.WaitForDisplayed();
            elementFactory.FindElements<Label>(HiddenElementsLoc, expectedCount: count, state: state);
            Assert.Throws<WebDriverTimeoutException>(
                () => elementFactory.FindElements<Label>(HiddenElementsLoc, expectedCount: count, state: state),
                $"Tried to find elements with expected count '{count}' and state '{state}'");
        }

        [TestCase(ElementsCount.MoreThenZero, ElementState.Displayed)]
        [TestCase(ElementsCount.MoreThenZero, ElementState.ExistsInAnyState)]
        public void Should_BeImpossibleTo_FindNotExistElements_WithWrongArguments(ElementsCount count, ElementState state)
        {
            Assert.Throws<WebDriverTimeoutException>(
                () => elementFactory.FindElements<Label>(NotExistElementLoc, expectedCount: count, state: state),
                $"Tried to find elements with expected count '{count}' and state '{state}'");
        }
    }
}
