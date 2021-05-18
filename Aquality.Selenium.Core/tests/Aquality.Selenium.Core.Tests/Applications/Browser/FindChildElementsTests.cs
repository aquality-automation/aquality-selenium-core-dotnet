using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Tests.Applications.Browser.Elements;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class FindChildElementsTests : FindElementsTests
    {
        private readonly Label customParent = new Label(By.XPath("//div[contains(@class,'figure')]"), 
            "custom parent", ElementState.ExistsInAnyState);
        protected override By HiddenElementsLoc => By.XPath(".//h5");
        protected override By DisplayedElementsLoc => By.XPath(".//img[@alt='User Avatar']");
        protected override By NotExistElementLoc => By.XPath(".//div[@class='testtest']");

        private static readonly By[] SupportedLocators = new By[]
        {
            By.XPath("//img"),
            By.XPath(".//img"),
            By.TagName("img")
        };

        protected override IList<T> FindElements<T>(By locator, string name = null, ElementSupplier<T> supplier = null, 
            ElementsCount expectedCount = ElementsCount.Any, ElementState state = ElementState.Displayed)
        {
            return ParentElement.FindChildElements(locator, name, supplier, expectedCount, state);
        }

        [Test]
        public void Should_GetCorrectNumberOfChilds_ForRelativeChildLocator(
            [ValueSource(nameof(SupportedLocators))] By childRelativeLocator)
        {
            var expectedCount = 3;
            var elementsCount = ElementFactory.FindChildElements<Label>(customParent, childRelativeLocator).Count;
            Assert.AreEqual(expectedCount, elementsCount, 
                $"Elements count for relative locator [{childRelativeLocator}] should be {expectedCount}");
        }

        [Test]
        public void Should_SetWorkableLocators_ToChildElements([ValueSource(nameof(SupportedLocators))] By childRelativeLocator)
        {
            var foundElements = ElementFactory.FindChildElements<Label>(customParent, childRelativeLocator);
            Assert.DoesNotThrow(() =>
            {
                foundElements.Select(element => element.GetElement()).ToList();
            });
        }

        [Test]
        public void Should_SetXPathLocator_InFindChildElement_IfBothParentAndChildHaveSupportedLocators(
            [ValueSource(nameof(SupportedLocators))] By childRelativeLocator)
        {
            var childLocatorString = customParent.FindChildElement<Label>(childRelativeLocator).Locator.ToString();
            CheckChildLocatorIsXpathAndStartsFromParent(childLocatorString);
        }

        [Test]
        public void Should_SetXPathLocator_InFindChildElements_IfBothParentAndChildHaveSupportedLocators(
            [ValueSource(nameof(SupportedLocators))] By childRelativeLocator)
        {
            var childLocatorStrings = customParent.FindChildElements<Label>(childRelativeLocator)
                .Select(child => child.Locator.ToString())
                .ToList();
            Assert.Multiple(() => childLocatorStrings.ForEach(CheckChildLocatorIsXpathAndStartsFromParent));            
        }

        private void CheckChildLocatorIsXpathAndStartsFromParent(string childLocatorString)
        {
            StringAssert.StartsWith("By.XPath", childLocatorString);
            var parentLocatorString = customParent.Locator.ToString();
            StringAssert.Contains(parentLocatorString.Substring(parentLocatorString.IndexOf(':') + 1).Trim(), childLocatorString);
        }
    }
}
