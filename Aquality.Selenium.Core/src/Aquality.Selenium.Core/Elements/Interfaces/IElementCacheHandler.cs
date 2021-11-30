using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;

namespace Aquality.Selenium.Core.Elements.Interfaces
{
    /// <summary>
    /// Allows to use cached element.
    /// </summary>
    public interface IElementCacheHandler
    {
        /// <summary>
        /// Determines is the cached element refresh needed.
        /// </summary>
        /// <param name="customState">Custom element's existance state used for search.</param>
        /// <returns></returns>
        bool IsRefreshNeeded(ElementState? customState = null);

        /// <summary>
        /// Determines is the element stale.
        /// </summary>
        bool IsStale { get; }

        /// <summary>
        /// Allows to get cached element.
        /// </summary>
        /// <param name="timeout">Timeout used to retrive the element when <see cref="IsRefreshNeeded(ElementState?)"/> is true.</param>
        /// <param name="customState">Custom element's existance state used for search.</param>
        /// <returns>Cached element.</returns>
        IWebElement GetElement(TimeSpan? timeout = null, ElementState? customState = null);
    }
}