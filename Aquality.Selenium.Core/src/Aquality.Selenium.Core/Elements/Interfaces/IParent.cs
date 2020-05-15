using OpenQA.Selenium;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Elements.Interfaces
{
    /// <summary>
    /// Defines behavior of element with child elements.
    /// </summary>
    public interface IParent
    {
        /// <summary>
        /// Finds child element of current element by its locator.
        /// </summary>
        /// <typeparam name="T">Type of child element that has to implement IElement.</typeparam>
        /// <param name="childLocator">Locator of child element.</param>
        /// <param name="name">Child element name.</param>
        /// <param name="supplier">Delegate that defines constructor of child element in case of custom element.</param>
        /// <param name="state">Child element state.</param>
        /// <returns>Instance of child element.</returns>
        T FindChildElement<T>(By childLocator, string name = null, ElementSupplier<T> supplier = null, ElementState state = ElementState.Displayed) where T : IElement;
        
        /// <summary>
        /// Finds child elements of current element by its locator.
        /// </summary>
        /// <typeparam name="T">Type of child elements that has to implement IElement.</typeparam>
        /// <param name="childLocator">Locator of child elements relative to their parent.</param>
        /// <param name="name">Child elements name.</param>
        /// <param name="supplier">Delegate that defines constructor of child element in case of custom element type.</param>
        /// <param name="state">Child elements state.</param>
        /// <returns>List of child elements.</returns>
        IList<T> FindChildElements<T>(By childLocator, string name = null, ElementSupplier<T> supplier = null, ElementsCount expectedCount = ElementsCount.Any, ElementState state = ElementState.Displayed) where T : IElement;
    }
}
