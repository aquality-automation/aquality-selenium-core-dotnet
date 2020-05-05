using System;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Utilities
{
    /// <summary>
    /// Retries an action or function when handledExceptions occurs.
    /// </summary>
    public interface IActionRetrier
    {
        /// <summary>
        /// Retries the action when the handled exception <see cref="HandledExceptions"/> occures.
        /// </summary>
        /// <param name="action">Action to be applied.</param>
        /// <param name="handledExceptions">Exceptions to be handled.</param>
        void DoWithRetry(Action action, IEnumerable<Type> handledExceptions);

        /// <summary>
        /// Retries the function when the handled exception <see cref="HandledExceptions"/> occures.
        /// </summary>
        /// <typeparam name="T">Return type of function.</typeparam>
        /// <param name="function">Function to be applied.</param>
        /// <param name="handledExceptions">Exceptions to be handled.</param>
        /// <returns>Result of the function.</returns>
        T DoWithRetry<T>(Func<T> function, IEnumerable<Type> handledExceptions);
    }
}
