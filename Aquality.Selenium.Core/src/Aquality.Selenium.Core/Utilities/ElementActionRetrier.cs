using Aquality.Selenium.Core.Configurations;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Utilities
{
    /// <summary>
    /// Retries an action or function when <see cref="HandledExceptions"/> occures.
    /// </summary>
    public class ElementActionRetrier : ActionRetrier, IElementActionRetrier
    {
        /// <summary>
        /// Instantiates the class using retry configuration.
        /// </summary>
        /// <param name="retryConfiguration">Retry configuration.</param>
        public ElementActionRetrier(IRetryConfiguration retryConfiguration) : base(retryConfiguration)
        {
        }

        /// <summary>
        /// Exceptions to be ignored during action retrying.
        /// Set by the constructor parameter.
        /// If were not passed to constructor, <see cref="StaleElementReferenceException"/> and <see cref="InvalidElementStateException"/> will be handled.
        /// </summary>
        public virtual IList<Type> HandledExceptions => new List<Type> { typeof(StaleElementReferenceException), typeof(InvalidElementStateException) };

        /// <summary>
        /// Retries the action when the handled exception <see cref="HandledExceptions"/> occures.
        /// </summary>
        /// <param name="action">Action to be applied.</param>
        public virtual void DoWithRetry(Action action)
        {
            DoWithRetry(action, HandledExceptions);
        }

        /// <summary>
        /// Retries the function when <see cref="HandledExceptions"/> occures.
        /// </summary>
        /// <typeparam name="T">Return type of function.</typeparam>
        /// <param name="function">Function to be applied.</param>
        /// <returns>Result of the function.</returns>
        public virtual T DoWithRetry<T>(Func<T> function)
        {
            return DoWithRetry(function, HandledExceptions);
        }
    }
}
