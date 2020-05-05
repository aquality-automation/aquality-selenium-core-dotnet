using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Utilities
{
    /// <summary>
    /// Retries an action or function when one of <see cref="HandledExceptions"/> occures.
    /// </summary>
    public interface IElementActionRetrier : IActionRetrier
    {
        /// <summary>
        /// Exceptions to be ignored during action retrying.
        /// By the default implementation, <see cref="StaleElementReferenceException"/> and <see cref="InvalidElementStateException"/> are handled.
        /// </summary>
        IList<Type> HandledExceptions { get; }

        /// <summary>
        /// Retries the action when the handled exception <see cref="HandledExceptions"/> occures.
        /// </summary>
        /// <param name="action">Action to be applied.</param>
        void DoWithRetry(Action action);

        /// <summary>
        /// Retries the function when the handled exception <see cref="HandledExceptions"/> occures.
        /// </summary>
        /// <typeparam name="T">Return type of function.</typeparam>
        /// <param name="function">Function to be applied.</param>
        /// <returns>Result of the function.</returns>
        T DoWithRetry<T>(Func<T> function);
    }
}