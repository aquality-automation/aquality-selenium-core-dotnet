using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Waitings;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace Aquality.Selenium.Core.Elements
{
    /// <summary>
    /// Factory that creates elements.
    /// </summary>
    public class ElementFactory : IElementFactory
    {
        private const string ByXpathIdentifier = "By.XPath";

        public ElementFactory(ConditionalWait conditionalWait, IElementFinder elementFinder, LocalizationManager localizationManager)
        {
            ConditionalWait = conditionalWait;
            ElementFinder = elementFinder;
            LocalizationManager = localizationManager;
        }

        protected ConditionalWait ConditionalWait { get; }

        protected IElementFinder ElementFinder { get; }

        protected LocalizationManager LocalizationManager { get; }

        /// <summary>
        /// Gets map between elements interfaces and their implementations.
        /// Can be extended for custom elements with custom interfaces.
        /// </summary>
        /// <returns>Dictionary where key is interface and value is its implementation.</returns>
        protected virtual IDictionary<Type, Type> ElementTypesMap => new Dictionary<Type, Type>();

        public T FindChildElement<T>(IElement parentElement, By childLocator, ElementSupplier<T> supplier = null, ElementState state = ElementState.Displayed) where T : IElement
        {
            var elementSupplier = ResolveSupplier(supplier);
            return elementSupplier(new ByChained(parentElement.Locator, childLocator), $"Child element of {parentElement.Name}", state);
        }

        public IList<T> FindElements<T>(By locator, ElementSupplier<T> supplier = null, ElementsCount expectedCount = ElementsCount.MoreThenZero, ElementState state = ElementState.Displayed) where T : IElement
        {
            var elementSupplier = ResolveSupplier(supplier);
            switch (expectedCount)
            {
                case ElementsCount.Zero:
                    ConditionalWait.WaitFor(driver => !driver.FindElements(locator).Any(
                        webElement => state == ElementState.ExistsInAnyState || webElement.Displayed),
                        message: LocalizationManager.GetLocalizedMessage("loc.elements.found.but.should.not", locator.ToString(), state.ToString()));
                    break;
                case ElementsCount.MoreThenZero:
                    ConditionalWait.WaitFor(driver => driver.FindElements(locator).Any(),
                        message: LocalizationManager.GetLocalizedMessage("loc.no.elements.found.by.locator", locator.ToString()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"No such expected value: {expectedCount}");
            }

            var webElements = ElementFinder.FindElements(locator, state, TimeSpan.Zero);
            IEnumerable<T> elements = webElements.Select((webElement, index) =>
            {
                var elementIndex = index + 1;
                return elementSupplier(GenerateXpathLocator(locator, webElement, elementIndex), $"element {elementIndex}", state);
            });
            return elements.ToList();
        }

        public T GetCustomElement<T>(ElementSupplier<T> elementSupplier, By locator, string name, ElementState state = ElementState.Displayed) where T : IElement
        {
            return elementSupplier(locator, name, state);
        }

        /// <summary>
        /// Generates xpath locator for target element
        /// </summary>
        /// <param name="baseLocator">locator of parent element</param>
        /// <param name="webElement">target element</param>
        /// <param name="elementIndex">index of target element</param>
        /// <returns>target element's locator</returns>
        protected virtual By GenerateXpathLocator(By baseLocator, IWebElement webElement, int elementIndex)
        {
            var strBaseLocator = baseLocator.ToString();
            var elementLocator = strBaseLocator.Contains(ByXpathIdentifier)
                    ? $"({strBaseLocator.Split(':')[1].Trim()})[{elementIndex}]"
                    : throw new NotSupportedException($"Parent element's locator {baseLocator} is not {ByXpathIdentifier}, and is not supported yet");
            return By.XPath(elementLocator);
        }

        /// <summary>
        /// Resolves element supplier or return itself if it is not null
        /// </summary>
        /// <typeparam name="T">type of target element</typeparam>
        /// <param name="supplier">target element supplier</param>
        /// <returns>non-null element supplier</returns>
        protected virtual ElementSupplier<T> ResolveSupplier<T>(ElementSupplier<T> supplier) where T : IElement
        {
            if (supplier != null)
            {
                return supplier;
            }
            else
            {
                var type = typeof(T);
                var elementType = type.IsInterface ? ElementTypesMap[type] : type;
                var elementCntr = elementType.GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance,
                        null,
                        new[] { typeof(By), typeof(string), typeof(ElementState) },
                        null);
                if(elementCntr == null)
                {
                    throw new InvalidOperationException($"cannot resolve constructor with required arguments for type {type.FullName}." +
                        Environment.NewLine +
                        $"Either provide a non-null {nameof(ElementSupplier<T>)}, or extend {nameof(ElementFactory)} with {nameof(ElementTypesMap)} for required type");
                }
                return (locator, name, state) => (T)elementCntr.Invoke(new object[] { locator, name, state });
            }
        }
    }
}
