using Aquality.Selenium.Core.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Tests.Utilities
{
    public class ActionRetrierTests : RetrierTests
    {
        private IEnumerable<Type> HandledExceptions => new List<Type> { typeof(InvalidOperationException) };
        
        private IActionRetrier ActionRetrier => new ActionRetrier(RetryConfiguration);

        [Test]
        public void Retrier_ShouldWork_OnceIfMethodSucceeded()
        {
            Retrier_ShouldWork_OnceIfMethodSucceeded(() => ActionRetrier.DoWithRetry(() => Console.WriteLine(1), new List<Type>()));
        }

        [Test]
        public void Retrier_ShouldWork_OnceIfMethodSucceeded_WithReturnValue()
        {
            Retrier_ShouldWork_OnceIfMethodSucceeded(() => ActionRetrier.DoWithRetry(() => string.Empty, new List<Type>()));
        }

        [Test]
        public void Retrier_ShouldWait_PollingTimeBetweenMethodCalls()
        {
            var throwException = true;
            Retrier_ShouldWait_PollingIntervalBetweenMethodsCall(() =>
                    ActionRetrier.DoWithRetry(() => {
                        if (throwException)
                        {
                            throwException = false;
                            throw new InvalidOperationException();
                        }
                    }, HandledExceptions));
        }

        [Test]
        public void Retrier_ShouldWait_PollingTimeBetweenMethodCalls_WithReturnValue()
        {
            var throwException = true;
            Retrier_ShouldWait_PollingIntervalBetweenMethodsCall(() =>
                    ActionRetrier.DoWithRetry(() => {
                        if (throwException)
                        {
                            throwException = false;
                            throw new InvalidOperationException();
                        }
                        return string.Empty;
                    }, HandledExceptions));
        }

        [Test]
        public void Retrier_ShouldThrow_UnhandledException()
        {
            Assert.Throws<InvalidOperationException>(() => ActionRetrier.DoWithRetry(() => {
                throw new InvalidOperationException();
            }, new List<Type>()));
        }

        [Test]
        public void Retrier_ShouldThrow_UnhandledException_WithReturnValue()
        {
            Assert.Throws<InvalidOperationException>(() => ActionRetrier.DoWithRetry(() => {
                throw new InvalidOperationException();
#pragma warning disable CS0162 // Unreachable code detected
                return string.Empty;
#pragma warning restore CS0162 // Unreachable code detected
            }, new List<Type>()));
        }

        [Test]
        public void Retrier_ShouldWork_CorrectTimes()
        {
            var actualAttempts = 0;
            Retrier_ShouldWork_CorrectTimes(typeof(InvalidOperationException), ref actualAttempts, () =>
                    ActionRetrier.DoWithRetry(() => {
                        Logger.Info($"current attempt is {actualAttempts++}");
                        throw new InvalidOperationException();
                    }, HandledExceptions));
        }

        [Test]
        public void Retrier_ShouldWork_CorrectTimes_WithReturnValue()
        {
            var actualAttempts = 0;
            Retrier_ShouldWork_CorrectTimes(typeof(InvalidOperationException), ref actualAttempts, () =>
                    ActionRetrier.DoWithRetry(() => {
                        Logger.Info($"current attempt is {actualAttempts++}");
                        throw new InvalidOperationException();
#pragma warning disable CS0162 // Unreachable code detected
                        return string.Empty;
#pragma warning restore CS0162 // Unreachable code detected
                    }, HandledExceptions));
        }

        [Test]
        public void Retrier_Should_ReturnValue()
        {
            var returnValue = string.Empty;
            Assert.AreEqual(returnValue, ActionRetrier.DoWithRetry(() => returnValue, new List<Type>()), "Retrier should return value");
        }
    }
}
