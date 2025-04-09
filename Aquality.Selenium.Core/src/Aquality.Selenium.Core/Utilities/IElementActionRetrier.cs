using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Utilities
{
    /// <summary>
    /// Retries an action or function when one of <see cref="HandledExceptions"/> occurs.
    /// </summary>
    public interface IElementActionRetrier : IActionRetrier
    {
        /// <summary>
        /// Exceptions to be ignored during action retrying.
        /// By the default implementation, <see cref="StaleElementReferenceException"/> 
        /// and <see cref="InvalidElementStateException"/> are handled.
        /// </summary>
        IEnumerable<Type> HandledExceptions { get; set; }
    }
}