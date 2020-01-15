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
        bool IsRefreshNeeded { get; }

        /// <summary>
        /// Determines is the element stale.
        /// </summary>
        bool IsStale { get; }

        /// <summary>
        /// Allows to get cached element.
        /// </summary>
        /// <param name="timeout">Timeout used to retrive the element when <see cref="IsRefreshNeeded"/> is true.</param>
        /// <returns>Cached element.</returns>
        RemoteWebElement GetElement(TimeSpan? timeout = null);
    }
}