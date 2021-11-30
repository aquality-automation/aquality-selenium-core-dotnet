using Aquality.Selenium.Core.Visualization;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;

namespace Aquality.Selenium.Core.Elements.Interfaces
{
    /// <summary>
    /// Describes behavior of any UI element.
    /// </summary>
    public interface IElement : IParent
    {
        /// <summary>
        /// Unique locator of element.
        /// </summary>
        /// <value>Instance of <see cref="By"/></value>
        By Locator { get; }

        /// <summary>
        /// Unique name of element.
        /// </summary>
        /// <value>String representation of element name.</value>
        string Name { get; }

        /// <summary>
        /// Gets element state.
        /// </summary>
        /// <value>Instance of <see cref="IElementStateProvider"/></value>
        IElementStateProvider State { get; }

        /// <summary>
        /// Gets element visual state.
        /// </summary>
        IVisualStateProvider Visual { get; }

        /// <summary>
        /// Finds current element by specified <see cref="Locator"/>
        /// </summary>
        /// <param name="timeout">Timeout to find element. Default: <see cref="Configurations.ITimeoutConfiguration.Condition"/></param>
        /// <returns>Instance of <see cref="IWebElement"/> if found.</returns>
        /// <exception cref="NoSuchElementException">Thrown when no elements found.</exception>
        IWebElement GetElement(TimeSpan? timeout = null);
                
        /// <summary>
        /// Gets element text.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Gets element attribute value by its name.
        /// </summary>
        /// <param name="attr">Name of attribute</param>
        /// <returns>Value of element attribute.</returns>
        string GetAttribute(string attr);

        /// <summary>
        /// Sends keys to element.
        /// </summary>
        /// <param name="key">Key to send.</param>
        void SendKeys(string key);

        /// <summary>
        /// Clicks the element.
        /// </summary>
        void Click();
    }
}
