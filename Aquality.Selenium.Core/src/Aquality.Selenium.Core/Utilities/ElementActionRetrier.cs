using Aquality.Selenium.Core.Configurations;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Aquality.Selenium.Core.Utilities
{
    /// <summary>
    /// Retries an action or function when <see cref="HandledExceptions"/> occures.
    /// </summary>
    public class ElementActionRetrier : IElementActionRetrier
    {
        private readonly IRetryConfiguration retryConfiguration;

        /// <summary>
        /// Instantiates the class using retry configuration.
        /// </summary>
        /// <param name="retryConfiguration">Retry configuration.</param>
        /// <param name="handledExceptions">Exceptions to be handled. 
        /// If set to null, <see cref="StaleElementReferenceException"/> and <see cref="InvalidElementStateException"/> will be handled.</param>
        public ElementActionRetrier(IRetryConfiguration retryConfiguration, IList<Type> handledExceptions = null)
        {
            this.retryConfiguration = retryConfiguration;
            HandledExceptions = handledExceptions ?? new List<Type> { typeof(StaleElementReferenceException), typeof(InvalidElementStateException) };
        }

        /// <summary>
        /// Exceptions to be ignored during action retrying.
        /// Set by the constructor parameter.
        /// If were not passed to constructor, <see cref="StaleElementReferenceException"/> and <see cref="InvalidElementStateException"/> will be handled.
        /// </summary>
        public virtual IList<Type> HandledExceptions { get; }

        /// <summary>
        /// Retries the action when the handled exception <see cref="HandledExceptions"/> occures.
        /// </summary>
        /// <param name="action">Action to be applied.</param>
        public virtual void DoWithRetry(Action action)
        {
            DoWithRetry(() =>
            {
                action();
                return true;
            });
        }

        /// <summary>
        /// Retries the function when <see cref="HandledExceptions"/> occures.
        /// </summary>
        /// <typeparam name="T">Return type of function.</typeparam>
        /// <param name="function">Function to be applied.</param>
        /// <returns>Result of the function.</returns>
        public virtual T DoWithRetry<T>(Func<T> function)
        {
            var retryAttemptsLeft = retryConfiguration.Number;
            var actualInterval = retryConfiguration.PollingInterval;
            var result = default(T);
            while (retryAttemptsLeft >= 0)
            {
                try
                {
                    result = function();
                    break;
                }
                catch (Exception exception)
                {
                    if (IsExceptionHandled(exception) && retryAttemptsLeft != 0)
                    {
                        Thread.Sleep(actualInterval);
                        retryAttemptsLeft--;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return result;
        }

        private bool IsExceptionHandled(Exception exception)
        {
            return HandledExceptions.Any(type => type.IsAssignableFrom(exception.GetType()));
        }
    }
}
