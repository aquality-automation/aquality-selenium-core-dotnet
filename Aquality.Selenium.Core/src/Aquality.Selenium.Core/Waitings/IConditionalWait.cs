using Aquality.Selenium.Core.Configurations;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Waitings
{
    /// <summary>
    /// Utility used to wait for some condition.
    /// </summary>
    public interface IConditionalWait
    {
        /// <summary>
        /// Wait for some condition within timeout.
        /// </summary>
        /// <param name="condition">Predicate for waiting</param>
        /// <param name="timeout">Condition timeout. Default value is <see cref="ITimeoutConfiguration.Condition"/></param>
        /// <param name="pollingInterval">Condition check interval. Default value is <see cref="ITimeoutConfiguration.PollingInterval"/></param>
        /// <param name="exceptionsToIgnore">Possible exceptions that have to be ignored. </param>
        /// <returns>True if condition satisfied and false otherwise.</returns>
        bool WaitFor(Func<bool> condition, TimeSpan? timeout = null, TimeSpan? pollingInterval = null, IList<Type> exceptionsToIgnore = null);
        
        /// <summary>
        /// Wait for some object from condition with timeout using Selenium WebDriver.
        /// </summary>
        /// <typeparam name="T">Type of object which is waiting for</typeparam>
        /// <param name="condition">Function for waiting</param>
        /// <param name="timeout">Condition timeout. Default value is <see cref="ITimeoutConfiguration.Condition"/></param>
        /// <param name="pollingInterval">Condition check interval. Default value is <see cref="ITimeoutConfiguration.PollingInterval"/></param>
        /// <param name="message">Part of error message in case of Timeout exception</param>
        /// <param name="exceptionsToIgnore">Possible exceptions that have to be ignored. Handles <see cref="StaleElementReferenceException"/> by default.</param>
        /// <returns>Condition result which is waiting for.</returns>
        /// <exception cref="WebDriverTimeoutException">Throws when timeout exceeded and condition not satisfied.</exception>
        T WaitFor<T>(Func<IWebDriver, T> condition, TimeSpan? timeout = null, TimeSpan? pollingInterval = null, string message = null, IList<Type> exceptionsToIgnore = null);

        /// <summary>
        /// Wait for some condition within timeout.
        /// </summary>
        /// <param name="condition">Predicate for waiting</param>
        /// <param name="timeout">Condition timeout. Default value is <see cref="ITimeoutConfiguration.Condition"/></param>
        /// <param name="pollingInterval">Condition check interval. Default value is <see cref="ITimeoutConfiguration.PollingInterval"/></param>
        /// <param name="message">Part of error message in case of Timeout exception</param>
        /// <param name="exceptionsToIgnore">Possible exceptions that have to be ignored. </param>
        /// <exception cref="TimeoutException">Throws when timeout exceeded and condition not satisfied.</exception>
        void WaitForTrue(Func<bool> condition, TimeSpan? timeout = null, TimeSpan? pollingInterval = null, string message = null, IList<Type> exceptionsToIgnore = null);
    }
}