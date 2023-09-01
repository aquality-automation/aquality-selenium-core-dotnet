using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Waitings;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Aquality.Selenium.Core.Elements
{
    /// <summary>
    /// Factory that creates elements.
    /// </summary>
    public class ElementFactory : IElementFactory
    {
        private const string ByXpathIdentifier = "By.XPath";
        private const string ByTagNameIdentifier = "By.TagName";
        private const string TagNameXPathPrefix = "//";

        public ElementFactory(IConditionalWait conditionalWait, IElementFinder elementFinder, ILocalizationManager localizationManager)
        {
            ConditionalWait = conditionalWait;
            ElementFinder = elementFinder;
            LocalizationManager = localizationManager;
        }

        protected IConditionalWait ConditionalWait { get; }

        protected IElementFinder ElementFinder { get; }

        protected ILocalizationManager LocalizationManager { get; }

        /// <summary>
        /// Gets map between elements interfaces and their implementations.
        /// Can be extended for custom elements with custom interfaces.
        /// </summary>
        /// <returns>Dictionary where key is interface and value is its implementation.</returns>
        protected virtual IDictionary<Type, Type> ElementTypesMap => new Dictionary<Type, Type>();

        public virtual T FindChildElement<T>(IElement parentElement, By childLocator, string name = null, ElementSupplier<T> supplier = null, ElementState state = ElementState.Displayed) where T : IElement
        {
            var elementSupplier = ResolveSupplier(supplier);
            return elementSupplier(GenerateAbsoluteChildLocator(parentElement.Locator, childLocator), name ?? $"Child element of {parentElement.Name}", state);
        }

        public virtual IList<T> FindChildElements<T>(IElement parentElement, By childLocator, string name = null, ElementSupplier<T> supplier = null, ElementsCount expectedCount = ElementsCount.Any, ElementState state = ElementState.Displayed) where T : IElement
        {
            return FindElements(GenerateAbsoluteChildLocator(parentElement.Locator, childLocator), name ?? $"Child element of {parentElement.Name}", supplier, expectedCount, state);
        }

        public virtual IList<T> FindElements<T>(By locator, string name = null, ElementSupplier<T> supplier = null, ElementsCount expectedCount = ElementsCount.Any, ElementState state = ElementState.Displayed) where T : IElement
        {
            var elementSupplier = ResolveSupplier(supplier);
            switch (expectedCount)
            {
                case ElementsCount.Zero:
                    ConditionalWait.WaitForTrue(() => !ElementFinder.FindElements(locator, state, TimeSpan.Zero, name).Any(), 
                        message: LocalizationManager.GetLocalizedMessage("loc.elements.with.name.found.but.should.not", name, locator.ToString(), state.ToString()));
                    break;
                case ElementsCount.MoreThenZero:
                    ConditionalWait.WaitForTrue(() => ElementFinder.FindElements(locator, state, TimeSpan.Zero, name).Any(), 
                        message: LocalizationManager.GetLocalizedMessage("loc.no.elements.with.name.found.by.locator", name, locator.ToString()));
                    break;
                case ElementsCount.Any:
                    ConditionalWait.WaitFor(() => ElementFinder.FindElements(locator, state, TimeSpan.Zero, name) != null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"No such expected value: {expectedCount}");
            }

            var webElements = ElementFinder.FindElements(locator, state, TimeSpan.Zero, name);
            IEnumerable<T> elements = webElements.Select((webElement, index) =>
            {
                var elementIndex = index + 1;
                var elementName = $"{name ?? "element"} {elementIndex}";
                return elementSupplier(GenerateLocator(locator, webElement, elementIndex), elementName, state);
            });
            return elements.ToList();
        }

        public virtual T GetCustomElement<T>(ElementSupplier<T> elementSupplier, By locator, string name, ElementState state = ElementState.Displayed) where T : IElement
        {
            return elementSupplier(locator, name, state);
        }

        /// <summary>
        /// Generates locator for target element
        /// </summary>
        /// <param name="baseLocator">locator of parent element</param>
        /// <param name="webElement">target element</param>
        /// <param name="elementIndex">index of target element</param>
        /// <returns>target element's locator</returns>
        protected virtual By GenerateLocator(By baseLocator, IWebElement webElement, int elementIndex)
        {
            return GenerateXpathLocator(baseLocator, webElement, elementIndex);
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
            var elementLocator = IsLocatorSupportedForXPathExtraction(baseLocator)
                    ? $"({ExtractXPathFromLocator(baseLocator)})[{elementIndex}]"
                    : throw new NotSupportedException($"Multiple elements' baseLocator type {baseLocator} is not supported yet");
            return By.XPath(elementLocator);
        }

        /// <summary>
        /// Generates absolute child locator for target element.
        /// </summary>
        /// <param name="parentLocator">parent locator</param>
        /// <param name="childLocator">child locator relative to parent</param>
        /// <returns>absolute locator of the child</returns>
        protected virtual By GenerateAbsoluteChildLocator(By parentLocator, By childLocator)
        {
            if (IsLocatorSupportedForXPathExtraction(parentLocator) && IsLocatorSupportedForXPathExtraction(childLocator))
            {
                var childLocatorString = ExtractXPathFromLocator(childLocator);
                var parentLocatorString = ExtractXPathFromLocator(parentLocator);
                return By.XPath(parentLocatorString +
                    $"{(childLocatorString.StartsWith(".") ? childLocatorString.Substring(1) : childLocatorString)}");
            }
            return new ByChained(parentLocator, childLocator);
        }

        /// <summary>
        /// Extracts XPath from passed locator.
        /// Current implementation works only with ByXPath.class and ByTagName locator types,
        /// but you can implement your own for the specific WebDriver type.
        /// </summary>
        /// <param name="locator">locator to get xpath from.</param>
        /// <returns>extracted XPath.</returns>
        protected virtual string ExtractXPathFromLocator(By locator)
        {
            var stringLocator = locator.ToString();
            string getLocatorWithoutPrefix() => stringLocator.Substring(stringLocator.IndexOf(':') + 1).Trim();
            if (stringLocator.StartsWith(ByXpathIdentifier))
            {
                return getLocatorWithoutPrefix();
            }
            if (stringLocator.StartsWith(ByTagNameIdentifier))
            {
                return $"{TagNameXPathPrefix}{getLocatorWithoutPrefix()}";
            }

            throw new NotSupportedException($"Cannot define xpath from locator {stringLocator}. Locator type is not supported yet");
        }

        /// <summary>
        /// Defines is the locator can be transformed to xpath or not.
        /// Current implementation works only with ByXPath.class and ByTagName locator types,
        /// but you can implement your own for the specific WebDriver type.
        /// </summary>
        /// <param name="locator">locator to transform</param>
        /// <returns>true if the locator can be transformed to xpath, false otherwise.</returns>
        protected virtual bool IsLocatorSupportedForXPathExtraction(By locator)
        {
            return locator.ToString().StartsWith(ByXpathIdentifier) || locator.ToString().StartsWith(ByTagNameIdentifier);
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
