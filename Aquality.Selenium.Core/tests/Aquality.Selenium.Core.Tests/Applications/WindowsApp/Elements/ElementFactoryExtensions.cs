using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements
{
    public static class ElementFactoryExtensions
    {
        public static Button GetButton(this IElementFactory elementFactory, By elementLocator, string elementName)
        {
            return elementFactory.GetCustomElement(GetButtonSupplier(), elementLocator, elementName);
        }

        public static Button FindChildButton(this IElementFactory elementFactory, IElement parentElement, By elementLocator, string name = null)
        {
            return elementFactory.FindChildElement(parentElement, elementLocator, name, GetButtonSupplier());
        }

        public static IList<Button> FindButtons(this IElementFactory elementFactory, By elementLocator, string name = null, ElementsCount elementsCount = ElementsCount.MoreThenZero)
        {
            return elementFactory.FindElements(elementLocator, name: name, GetButtonSupplier(), elementsCount);
        }

        private static ElementSupplier<Button> GetButtonSupplier()
        {
            return (locator, name, state) => new Button(locator, name);
        }
    }
}
