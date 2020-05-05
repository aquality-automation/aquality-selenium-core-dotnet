using Aquality.Selenium.Core.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Aquality.Selenium.Core.Utilities
{
    public class ActionRetrier : IActionRetrier
    {
        private readonly IRetryConfiguration retryConfiguration;

        /// <summary>
        /// Instantiates the class using retry configuration.
        /// </summary>
        /// <param name="retryConfiguration">Retry configuration.</param>
        public ActionRetrier(IRetryConfiguration retryConfiguration)
        {
            this.retryConfiguration = retryConfiguration;
        }

        /// <summary>
        /// Retries the action when one of the handledExceptions occures.
        /// </summary>
        /// <param name="action">Action to be applied.</param>
        /// <param name="handledExceptions">Exceptions to be handled.</param>
        public virtual void DoWithRetry(Action action, IEnumerable<Type> handledExceptions)
        {
            DoWithRetry(() =>
            {
                action();
                return true;
            }, handledExceptions);
        }

        /// <summary>
        /// Retries the action when one of the handledExceptions occures.
        /// </summary>
        /// <typeparam name="T">Return type of function.</typeparam>
        /// <param name="function">Function to be applied.</param>
        /// <param name="handledExceptions">Exceptions to be handled.</param>
        /// <returns>Result of the function.</returns>
        public virtual T DoWithRetry<T>(Func<T> function, IEnumerable<Type> handledExceptions)
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
                    if (IsExceptionHandled(handledExceptions, exception) && retryAttemptsLeft != 0)
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

        /// <summary>
        /// Decides should the occured exception be handled (ignored during the retry) or not.
        /// </summary>
        /// <param name="handledExceptions">Exceptions to be handled.</param>
        /// <param name="exception">Exception to proceed.</param>
        /// <returns>True if the exception should be ignored, false otherwise.</returns>
        protected virtual bool IsExceptionHandled(IEnumerable<Type> handledExceptions, Exception exception)
        {
            return handledExceptions.Any(type => type.IsAssignableFrom(exception.GetType()));
        }
    }
}
