using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;

namespace Aquality.Selenium.Core.Elements.Interfaces
{

    /// <summary>
    /// Provides ability to find elements in desired ElementState.
    /// </summary>
    public interface IElementFinder
    {
        /// <summary>
        /// Finds element in desired ElementState.
        /// </summary>
        /// <param name="locator">element locator</param>
        /// <param name="state">desired ElementState</param>
        /// <param name="timeout">timeout for search</param>
        /// <param name="name">element name to be used for logging and exception message</param>
        /// <exception cref="NoSuchElementException">Thrown if element was not found in time in desired state</exception> 
        /// <returns>Found element</returns>
        IWebElement FindElement(By locator, ElementState state = ElementState.ExistsInAnyState, TimeSpan? timeout = null, string name = null);

        /// <summary>
        /// Finds element in state defined by predicate.
        /// </summary>
        /// <param name="locator">elements locator</param>
        /// <param name="elementStateCondition">predicate to define element state</param>
        /// <param name="timeout">timeout for search</param>
        /// <param name="name">element name to be used for logging and exception message</param>
        /// <exception cref="NoSuchElementException">Thrown if element was not found in time in desired state</exception> 
        /// <returns>Found element</returns>
        IWebElement FindElement(By locator, Func<IWebElement, bool> elementStateCondition, TimeSpan? timeout = null, string name = null);

        /// <summary>
        /// Finds elements in desired ElementState.
        /// </summary>
        /// <param name="locator">elements locator</param>
        /// <param name="state">desired ElementState</param>
        /// <param name="timeout">timeout for search</param>
        /// <param name="name">elements' name to be used for logging and exception message</param>
        /// <returns>List of found elements</returns>
        ReadOnlyCollection<IWebElement> FindElements(By locator, ElementState state = ElementState.ExistsInAnyState, TimeSpan? timeout = null, string name = null);

        /// <summary>
        /// Finds elements in state defined by predicate.
        /// </summary>
        /// <param name="locator">elements locator</param>
        /// <param name="elementStateCondition">predicate to define elements state</param>
        /// <param name="timeout">timeout for search</param>
        /// <param name="name">elements' name to be used for logging and exception message</param>
        /// <returns>List of found elements</returns>
        ReadOnlyCollection<IWebElement> FindElements(By locator, Func<IWebElement, bool> elementStateCondition, TimeSpan? timeout = null, string name = null);

        /// <summary>
        /// Finds elements in state defined by desired state.
        /// </summary>
        /// <param name="locator">elements locator</param>
        /// <param name="desiredState">desired elements state</param>
        /// <param name="timeout">timeout for search</param>
        /// <param name="name">elements' name to be used for logging and exception message</param>
        /// <returns>List of found elements</returns>
        ReadOnlyCollection<IWebElement> FindElements(By locator, DesiredState desiredState, TimeSpan? timeout = null, string name = null);
    }
}
