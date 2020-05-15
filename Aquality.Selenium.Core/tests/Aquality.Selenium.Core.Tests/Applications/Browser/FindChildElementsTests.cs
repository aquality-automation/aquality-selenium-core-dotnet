using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class FindChildElementsTests : FindElementsTests
    {
        private readonly Label customParent = new Label(By.XPath("//div[contains(@class,'figure')][1]"), "custom parent", ElementState.ExistsInAnyState);
        protected override By HiddenElementsLoc => By.XPath(".//h5");
        protected override By DisplayedElementsLoc => By.XPath(".//img[@alt='User Avatar']");
        protected override By NotExistElementLoc => By.XPath(".//div[@class='testtest']");

        protected override IList<T> FindElements<T>(By locator, string name = null, ElementSupplier<T> supplier = null, ElementsCount expectedCount = ElementsCount.Any, ElementState state = ElementState.Displayed)
        {
            return elementFactory.FindChildElements(parentElement, locator, name, supplier, expectedCount, state);
        }

        [Test]
        public void Should_GetCorrectNumberOfChilds_ForRelativeChildLocator()
        {
            var expectedCount = 1;
            var elementsCount = elementFactory.FindChildElements<Label>(customParent, DisplayedElementsLoc).Count;
            Assert.AreEqual(expectedCount, elementsCount, $"Elements count for relative locator should be {expectedCount}");
        }

        [Test]
        public void Should_GetCorrectNumberOfChilds_ForAbsoluteChildLocator()
        {
            var expectedCount = 3;
            var elementsCount = elementFactory.FindChildElements<Label>(customParent, base.DisplayedElementsLoc).Count;
            Assert.AreEqual(expectedCount, elementsCount, $"Elements count for absolute locator should be {expectedCount}");
        }

        [Test]
        public void Should_SetWorkableLocators_ToChildElements()
        {
            var foundElements = elementFactory.FindChildElements<Label>(customParent, base.DisplayedElementsLoc);
            Assert.DoesNotThrow(() =>
            {
                foundElements.Select(element => element.GetElement()).ToList();
            });
        }
    }
}
