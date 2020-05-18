using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Elements.Interfaces
{
    /// <summary>
    /// Defines the interface used to create the elements.
    /// </summary>
    public interface IElementFactory
    {
        /// <summary>
        /// Create custom element according to passed parameters.
        /// </summary>
        /// <typeparam name="T">Type of the target element.</typeparam>
        /// <param name="elementSupplier">Delegate that defines constructor of element.</param>
        /// <param name="locator">Locator of the target element.</param>
        /// <param name="name">Name of the target element.</param>
        /// <param name="state">State of the target element.</param>
        /// <returns>Instance of custom element.</returns>
        T GetCustomElement<T>(ElementSupplier<T> elementSupplier, By locator, string name, ElementState state = ElementState.Displayed) where T : IElement;

        /// <summary>
        /// Finds child element by its locator relative to parent element.
        /// </summary>
        /// <typeparam name="T">Type of child element that has to implement IElement.</typeparam>
        /// <param name="parentElement">Parent element.</param>
        /// <param name="childLocator">Locator of child element relative to its parent.</param>
        /// <param name="supplier">Delegate that defines constructor of element in case of custom element.</param>
        /// <param name="state">Child element state.</param>
        /// <param name="name">Child element name.</param>
        /// <exception cref="InvalidOperationException">Thrown when the supplier is null, and no constructor with required arguments was found.</exception>
        /// <returns>Instance of child element</returns>
        T FindChildElement<T>(IElement parentElement, By childLocator, string name = null, ElementSupplier<T> supplier = null, ElementState state = ElementState.Displayed) where T : IElement;

        /// <summary>
        /// Finds list of child elements by their locator relative to parent element.
        /// </summary>
        /// <typeparam name="T">Type of child elements that has to implement IElement.</typeparam>
        /// <param name="parentElement">Parent element.</param>
        /// <param name="childLocator">Locator of child elements relative to their parent.</param>
        /// <param name="supplier">Delegate that defines constructor of element in case of custom element type.</param>
        /// <param name="expectedCount">Expected number of elements that have to be found (zero, more then zero, any).</param>
        /// <param name="state">Child elements state.</param>
        /// <param name="name">Child elements name.</param>
        /// <exception cref="InvalidOperationException">Thrown when the supplier is null, and no constructor with required arguments was found.</exception>
        /// <returns>List of child elements.</returns>
        IList<T> FindChildElements<T>(IElement parentElement, By childLocator, string name = null, ElementSupplier<T> supplier = null, ElementsCount expectedCount = ElementsCount.Any, ElementState state = ElementState.Displayed) where T : IElement;

        /// <summary>
        /// Finds list of elements by base locator.
        /// </summary>
        /// <typeparam name="T">Type of elements that has to implement IElement.</typeparam>
        /// <param name="locator">Base elements locator.</param>
        /// <param name="supplier">Delegate that defines constructor of element in case of custom elements.</param>
        /// <param name="expectedCount">Expected number of elements that have to be found (zero, more then zero, any).</param>
        /// <param name="state">Elements state.</param>
        /// <param name="name">Elements name.</param>
        /// <exception cref="InvalidOperationException">Thrown when the supplier is null, and no constructor with required arguments was found.</exception>
        /// <returns>List of elements that found by locator.</returns>
        IList<T> FindElements<T>(By locator, string name = null, ElementSupplier<T> supplier = null, ElementsCount expectedCount = ElementsCount.Any, ElementState state = ElementState.Displayed) where T : IElement;
    }
}
