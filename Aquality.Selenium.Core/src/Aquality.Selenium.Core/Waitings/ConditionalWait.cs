using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aquality.Selenium.Core.Waitings
{
    /// <summary>
    /// This class is used for waiting any conditions.
    /// </summary>
    public class ConditionalWait : IConditionalWait
    {
        private readonly ITimeoutConfiguration timeoutConfiguration;
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Instantiates the class using retry configuration.
        /// <paramref name="timeoutConfiguration"/>
        /// <paramref name="serviceProvider"/>
        /// </summary>        
        public ConditionalWait(ITimeoutConfiguration timeoutConfiguration, IServiceProvider serviceProvider)
        {
            this.timeoutConfiguration = timeoutConfiguration;
            this.serviceProvider = serviceProvider;
        }

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
        public T WaitFor<T>(Func<IWebDriver, T> condition, TimeSpan? timeout = null, TimeSpan? pollingInterval = null, string message = null, IList<Type> exceptionsToIgnore = null)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var application = scope.ServiceProvider.GetRequiredService<IApplication>();
                application.SetImplicitWaitTimeout(TimeSpan.Zero);
                var waitTimeout = ResolveConditionTimeout(timeout);
                var checkInterval = ResolvePollingInterval(pollingInterval);
                var wait = new WebDriverWait(application.Driver, waitTimeout)
                {
                    Message = message,
                    PollingInterval = checkInterval
                };
                var ignoreExceptions = exceptionsToIgnore ?? new List<Type> { typeof(StaleElementReferenceException) };
                wait.IgnoreExceptionTypes(ignoreExceptions.ToArray());
                var result = wait.Until(condition);
                application.SetImplicitWaitTimeout(timeoutConfiguration.Implicit);
                return result;
            }
        }

        /// <summary>
        /// Wait for some condition within timeout.
        /// </summary>
        /// <param name="condition">Predicate for waiting</param>
        /// <param name="timeout">Condition timeout. Default value is <see cref="ITimeoutConfiguration.Condition"/></param>
        /// <param name="pollingInterval">Condition check interval. Default value is <see cref="ITimeoutConfiguration.PollingInterval"/></param>
        /// <param name="exceptionsToIgnore">Possible exceptions that have to be ignored. </param>
        /// <returns>True if condition satisfied and false otherwise.</returns>
        public bool WaitFor(Func<bool> condition, TimeSpan? timeout = null, TimeSpan? pollingInterval = null, IList<Type> exceptionsToIgnore = null)
        {
            return IsConditionSatisfied(() =>
            {
                WaitForTrue(condition, timeout, pollingInterval, exceptionsToIgnore: exceptionsToIgnore);
                return true;
            }, new List<Type> { typeof(TimeoutException) });
        }

        /// <summary>
        /// Wait for some condition asynchronously within timeout.
        /// </summary>
        /// <param name="condition">Predicate for waiting</param>
        /// <param name="timeout">Condition timeout. Default value is <see cref="ITimeoutConfiguration.Condition"/></param>
        /// <param name="pollingInterval">Condition check interval. Default value is <see cref="ITimeoutConfiguration.PollingInterval"/></param>
        /// <param name="exceptionsToIgnore">Possible exceptions that have to be ignored. </param>
        /// <returns>A task that returns true if condition satisfied and false otherwise.</returns>
        public Task<bool> WaitForAsync(Func<bool> condition, TimeSpan? timeout = null, TimeSpan? pollingInterval = null, IList<Type> exceptionsToIgnore = null)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition), "condition cannot be null");
            }
            var waitTimeout = ResolveConditionTimeout(timeout);
            var checkInterval = ResolvePollingInterval(pollingInterval);
            return WaitForAsyncCore(condition, exceptionsToIgnore, waitTimeout, checkInterval);
        }

        /// <summary>
        /// Wait for some condition asynchronously within timeout.
        /// </summary>
        /// <param name="condition">Predicate for waiting</param>
        /// <param name="timeout">Condition timeout. Default value is <see cref="ITimeoutConfiguration.Condition"/></param>
        /// <param name="pollingInterval">Condition check interval. Default value is <see cref="ITimeoutConfiguration.PollingInterval"/></param>
        /// <param name="message">Part of error message in case of Timeout exception</param>
        /// <param name="exceptionsToIgnore">Possible exceptions that have to be ignored. </param>
        /// <exception cref="TimeoutException">Throws when timeout exceeded and condition not satisfied, if only the task is awaited.</exception>
        /// <returns>A task that throws a <see cref="TimeoutException"/> if condition is not satisfied after the timeout.</returns>
        public async Task WaitForTrueAsync(Func<bool> condition, TimeSpan? timeout = null, TimeSpan? pollingInterval = null, string message = null, IList<Type> exceptionsToIgnore = null)
        {
            var waitTimeout = ResolveConditionTimeout(timeout);
            if (!await WaitForAsync(condition, waitTimeout, pollingInterval, exceptionsToIgnore))
            {
                throw GetTimeoutException(waitTimeout, message);
            }
        }

        /// <summary>
        /// Wait for some condition within timeout.
        /// </summary>
        /// <param name="condition">Predicate for waiting</param>
        /// <param name="timeout">Condition timeout. Default value is <see cref="ITimeoutConfiguration.Condition"/></param>
        /// <param name="pollingInterval">Condition check interval. Default value is <see cref="ITimeoutConfiguration.PollingInterval"/></param>
        /// <param name="message">Part of error message in case of Timeout exception</param>
        /// <param name="exceptionsToIgnore">Possible exceptions that have to be ignored. </param>
        /// <exception cref="TimeoutException">Throws when timeout exceeded and condition not satisfied.</exception>
        public void WaitForTrue(Func<bool> condition, TimeSpan? timeout = null, TimeSpan? pollingInterval = null, string message = null, IList<Type> exceptionsToIgnore = null)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition), "condition cannot be null");
            }

            var waitTimeout = ResolveConditionTimeout(timeout);
            var checkInterval = ResolvePollingInterval(pollingInterval);
            var stopwatch = Stopwatch.StartNew();
            while (true)
            {
                if (IsConditionSatisfied(condition, exceptionsToIgnore ?? new List<Type>()))
                {
                    return;
                }

                if (stopwatch.Elapsed > waitTimeout)
                {
                    throw GetTimeoutException(waitTimeout, message);
                }

                Thread.Sleep(checkInterval);
            }
        }

        private TimeoutException GetTimeoutException(TimeSpan waitTimeout, string message)
        {
            var exceptionMessage = $"Timed out after {waitTimeout.TotalSeconds} seconds";
            if (!string.IsNullOrEmpty(message))
            {
                exceptionMessage += $": {message}";
            }

            return new TimeoutException(exceptionMessage);
        }

        private bool IsConditionSatisfied(Func<bool> condition, IList<Type> exceptionsToIgnore)
        {
            try
            {
                return condition();
            }
            catch (Exception exception)
            {
                if (exceptionsToIgnore.Any(type => type.IsAssignableFrom(exception.GetType())))
                {
                    return false;
                }

                throw;
            }
        }

        private TimeSpan ResolveConditionTimeout(TimeSpan? timeout)
        {
            return timeout ?? timeoutConfiguration.Condition;
        }

        private TimeSpan ResolvePollingInterval(TimeSpan? pollingInterval)
        {
            return pollingInterval ?? timeoutConfiguration.PollingInterval;
        }

        private async Task<bool> WaitForAsyncCore(Func<bool> condition, IList<Type> exceptionsToIgnore, TimeSpan waitTimeout, TimeSpan checkInterval)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                var token = cts.Token;
                var waitTask = Task.Run(async () =>
                {
                    while (!IsConditionSatisfied(condition, exceptionsToIgnore ?? new List<Type>()))
                    {
                        await Task.Delay(checkInterval);
                    }
                },
                token);
                var finishedTask = await Task.WhenAny(waitTask, Task.Delay(waitTimeout, token));
                cts.Cancel();
                var result = finishedTask == waitTask;
                if (result && waitTask.Exception != null)
                {
                    throw waitTask.Exception.InnerExceptions.Count == 1 ? waitTask.Exception.InnerException : waitTask.Exception;
                }
                return result;
            }
        }
    }
}
