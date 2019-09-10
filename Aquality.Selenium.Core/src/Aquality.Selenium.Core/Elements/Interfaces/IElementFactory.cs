using OpenQA.Selenium;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Elements.Interfaces
{
    /// <summary>
    /// Defines the interface used to create the elements.
    /// </summary>
    public interface IElementFactory
    {
        /// <summary>
        /// Create custom element according to passed parameters
        /// </summary>
        /// <typeparam name="T">Type of the target element</typeparam>
        /// <param name="elementSupplier">Delegate that defines constructor of element</param>
        /// <param name="locator">Locator of the target element</param>
        /// <param name="name">Name of the target element</param>
        /// <param name="state">State of the target element</param>
        /// <returns></returns>
        T GetCustomElement<T>(ElementSupplier<T> elementSupplier, By locator, string name, ElementState state = ElementState.Displayed) where T : IElement;

        /// <summary>
        /// Finds child element by its locator relative to parent element.
        /// </summary>
        /// <typeparam name="T">Type of child element that has to implement IElement</typeparam>
        /// <param name="parentElement">Parent element</param>
        /// <param name="childLocator">Locator of child element relative to its parent</param>
        /// <param name="supplier">Delegate that defines constructor of element in case of custom element</param>
        /// <param name="state">Child element state</param>
        /// <returns>Instance of child element</returns>
        T FindChildElement<T>(IElement parentElement, By childLocator, ElementSupplier<T> supplier = null, ElementState state = ElementState.Displayed) where T : IElement;

        /// <summary>
        /// Finds list of elements by base locator.
        /// </summary>
        /// <typeparam name="T">Type of elements that have to implement IElement</typeparam>
        /// <param name="locator">Base elements locator</param>
        /// <param name="supplier">Delegate that defines constructor of element in case of custom elements</param>
        /// <param name="expectedCount">Expected number of elements that have to be found (zero ot more then zero)</param>
        /// <param name="state">Elements state</param>
        /// <returns>List of elements that found by locator</returns>
        IList<T> FindElements<T>(By locator, ElementSupplier<T> supplier = null, ElementsCount expectedCount = ElementsCount.MoreThenZero, ElementState state = ElementState.Displayed) where T : IElement;
    }
}
