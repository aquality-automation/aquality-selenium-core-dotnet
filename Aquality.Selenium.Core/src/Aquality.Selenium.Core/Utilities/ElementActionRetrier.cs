using Aquality.Selenium.Core.Configurations;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Utilities
{
    /// <summary>
    /// Retries an action or function when <see cref="HandledExceptions"/> occurs.
    /// </summary>
    public class ElementActionRetrier : ActionRetrier, IElementActionRetrier
    {
        /// <summary>
        /// Instantiates the class using retry configuration.
        /// </summary>
        /// <param name="retryConfiguration">Retry configuration.</param>
        public ElementActionRetrier(IRetryConfiguration retryConfiguration) : base(retryConfiguration)
        {
            HandledExceptions = new List<Type>
            {
                typeof(StaleElementReferenceException),
                typeof(InvalidElementStateException)
            };
        }

        /// <summary>
        /// Exceptions to be ignored during action retrying.
        /// Set by the constructor parameter.
        /// If were not passed to constructor, <see cref="StaleElementReferenceException"/> 
        /// and <see cref="InvalidElementStateException"/> will be handled.
        /// </summary>
        public virtual IEnumerable<Type> HandledExceptions { get; set; }

        /// <summary>
        /// Retries the action when the handled exception <see cref="HandledExceptions"/> occurs.
        /// </summary>
        /// <param name="action">Action to be applied.</param>
        /// <param name="handledExceptions">Exceptions to be handled.</param>
        public override void DoWithRetry(Action action, IEnumerable<Type> handledExceptions = null)
        {
            var exceptionsToHandle = handledExceptions ?? HandledExceptions;
            base.DoWithRetry(action, exceptionsToHandle);
        }

        /// <summary>
        /// Retries the function when <see cref="HandledExceptions"/> occurs.
        /// </summary>
        /// <typeparam name="T">Return type of function.</typeparam>
        /// <param name="function">Function to be applied.</param>
        /// <param name="handledExceptions">Exceptions to be handled.</param>
        /// <returns>Result of the function.</returns>
        public override T DoWithRetry<T>(Func<T> function, IEnumerable<Type> handledExceptions = null)
        {
            var exceptionsToHandle = handledExceptions ?? HandledExceptions;
            return base.DoWithRetry(function, exceptionsToHandle);
        }
    }
}
