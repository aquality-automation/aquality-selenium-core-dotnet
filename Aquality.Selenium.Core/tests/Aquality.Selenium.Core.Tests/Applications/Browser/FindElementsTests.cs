using NUnit.Framework;
using System;
using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class FindElementsTests : TestWithBrowser
    {
        private static readonly By ContentLoc = By.XPath("//div[contains(@class,'example')]");
        private static readonly Uri HoversURL = new Uri($"{TestSite}/hovers");

        protected virtual By HiddenElementsLoc => By.XPath("//h5");
        protected virtual By DisplayedElementsLoc => By.XPath("//img[@alt='User Avatar']");
        protected virtual By DottedLoc => By.XPath("//img[@alt='User Avatar']/parent::div");
        protected virtual By NotExistElementLoc => By.XPath("//div[@class='testtest']");
        
        protected IElementFactory ElementFactory { get; private set; }
        protected Label ParentElement { get; private set; }

        protected virtual IList<T> FindElements<T>(By locator, string name = null, ElementSupplier<T> supplier = null, ElementsCount expectedCount = ElementsCount.Any, ElementState state = ElementState.Displayed) where T : IElement
        {
            return ElementFactory.FindElements(locator, name, supplier, expectedCount, state);
        }

        [SetUp]
        public new void SetUp()
        {
            ElementFactory = ServiceProvider.GetRequiredService<IElementFactory>();
            AqualityServices.Application.Driver.Navigate().GoToUrl(HoversURL);
            ParentElement = new Label(ContentLoc, "Example", ElementState.Displayed);
            ParentElement.Click();
        }

        [TestCase(ElementsCount.MoreThenZero, ElementState.Displayed, 3)]
        [TestCase(ElementsCount.MoreThenZero, ElementState.ExistsInAnyState, 3)]
        [TestCase(ElementsCount.Any, ElementState.Displayed, 3)]
        [TestCase(ElementsCount.Any, ElementState.ExistsInAnyState, 3)]
        public void Should_BePossibleTo_FindElements_ForDisplayedElements(ElementsCount count, ElementState state, int expectedCount)
        {
            var elementsCount = FindElements<Label>(DisplayedElementsLoc, expectedCount: count, state: state).Count;
            Assert.AreEqual(expectedCount, elementsCount, $"Elements count for displayed elements should be {expectedCount}");
        }

        [TestCase(ElementsCount.Zero, ElementState.Displayed, 0)]
        [TestCase(ElementsCount.MoreThenZero, ElementState.ExistsInAnyState, 3)]
        [TestCase(ElementsCount.Any, ElementState.Displayed, 0)]
        [TestCase(ElementsCount.Any, ElementState.ExistsInAnyState, 3)]
        public void Should_BePossibleTo_FindElements_ForHiddenElements(ElementsCount count, ElementState state, int expectedCount)
        {
            var elementsCount = FindElements<Label>(HiddenElementsLoc, expectedCount: count, state: state).Count;
            Assert.AreEqual(expectedCount, elementsCount, $"Elements count for hidden elements should be {expectedCount}");
        }

        [TestCase(ElementsCount.Zero, ElementState.Displayed, 0)]
        [TestCase(ElementsCount.Zero, ElementState.ExistsInAnyState, 0)]
        [TestCase(ElementsCount.Any, ElementState.Displayed, 0)]
        [TestCase(ElementsCount.Any, ElementState.ExistsInAnyState, 0)]
        public void Should_BePossibleTo_FindElements_ForNotExistsElements(ElementsCount count, ElementState state, int expectedCount)
        {
            var elementsCount = FindElements<Label>(NotExistElementLoc, expectedCount: count, state: state).Count;
            Assert.AreEqual(expectedCount, elementsCount, $"Elements count for not existing elements should be {expectedCount}");
        }

        [TestCase(ElementsCount.Zero, ElementState.Displayed)]
        [TestCase(ElementsCount.Zero, ElementState.ExistsInAnyState)]
        public void Should_BeImpossibleTo_FindDisplayedElements_WithWrongArguments(ElementsCount count, ElementState state)
        {
            Assert.Throws<TimeoutException>(
                () => FindElements<Label>(DisplayedElementsLoc, expectedCount: count, state: state),
                $"Tried to find elements with expected count '{count}' and state '{state}'");
        }

        [TestCase(ElementsCount.MoreThenZero, ElementState.Displayed)]
        [TestCase(ElementsCount.Zero, ElementState.ExistsInAnyState)]
        public void Should_BeImpossibleTo_FindHiddenElements_WithWrongArguments(ElementsCount count, ElementState state)
        {
            Assert.Throws<TimeoutException>(
                () => FindElements<Label>(HiddenElementsLoc, expectedCount: count, state: state),
                $"Tried to find elements with expected count '{count}' and state '{state}'");
        }

        [TestCase(ElementsCount.MoreThenZero, ElementState.Displayed)]
        [TestCase(ElementsCount.MoreThenZero, ElementState.ExistsInAnyState)]
        public void Should_BeImpossibleTo_FindNotExistElements_WithWrongArguments(ElementsCount count, ElementState state)
        {
            Assert.Throws<TimeoutException>(
                () => FindElements<Label>(NotExistElementLoc, expectedCount: count, state: state),
                $"Tried to find elements with expected count '{count}' and state '{state}'");
        }

        [Test]
        public void Should_BePossibleTo_WorkWithElements_FoundByDottedLocator()
        {
            var foundElements = FindElements<Label>(DottedLoc, expectedCount: ElementsCount.MoreThenZero);
            Assert.DoesNotThrow(
                () => foundElements.Select(element => element.GetElement()).ToList(),
                $"Failed to find elements using dotted locator [{DottedLoc}]");
        }
    }
}
