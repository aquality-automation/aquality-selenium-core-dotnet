using Aquality.Selenium.Core.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Tests.Utilities
{
    public class ElementActionRetrierTests : RetrierTests
    {
        private IElementActionRetrier ElementActionRetrier => new ElementActionRetrier(RetryConfiguration);

        private static readonly Exception[] handledExceptions =
        [
            new StaleElementReferenceException(""),
            new InvalidElementStateException("")
        ];

        [Test]
        public void Retrier_ShouldWork_OnceIfMethodSucceeded()
        {
            Retrier_ShouldWork_OnceIfMethodSucceeded(() => ElementActionRetrier.DoWithRetry(() => Console.WriteLine(1)));
        }

        [Test]
        public void Retrier_ShouldWork_OnceIfMethodSucceeded_WithReturnValue()
        {
            Retrier_ShouldWork_OnceIfMethodSucceeded(() => ElementActionRetrier.DoWithRetry(() => string.Empty));
        }

        [Test]
        public void Retrier_ShouldWait_PollingTimeBetweenMethodCalls([ValueSource(nameof(handledExceptions))] Exception exception)
        {
            var throwException = true;
            Retrier_ShouldWait_PollingIntervalBetweenMethodsCall(() =>
                    ElementActionRetrier.DoWithRetry(() => {
                        if (throwException)
                        {
                            throwException = false;
                            throw exception;
                        }
                    }));
        }

        [Test]
        public void Retrier_ShouldWait_PollingTimeBetweenMethodCalls_WithReturnValue([ValueSource(nameof(handledExceptions))] Exception exception)
        {
            var throwException = true;
            Retrier_ShouldWait_PollingIntervalBetweenMethodsCall(() =>
                    ElementActionRetrier.DoWithRetry(() => {
                        if (throwException)
                        {
                            throwException = false;
                            throw exception;
                        }
                        return string.Empty;
                    }));
        }

        [Test]
        public void Retrier_ShouldThrow_UnhandledException()
        {
            Assert.Throws<InvalidOperationException>(() => ElementActionRetrier.DoWithRetry(() => {
                throw new InvalidOperationException();
            }));
        }

        [Test]
        public void Retrier_ShouldThrow_UnhandledException_WithReturnValue()
        {
            Assert.Throws<InvalidOperationException>(() => ElementActionRetrier.DoWithRetry(() => {
                throw new InvalidOperationException();
#pragma warning disable CS0162 // Unreachable code detected
                return string.Empty;
#pragma warning restore CS0162 // Unreachable code detected
            }));
        }

        [Test]
        public void Retrier_ShouldWork_CorrectTimes([ValueSource(nameof(handledExceptions))] Exception exception)
        {
            var actualAttempts = 0;
            Retrier_ShouldWork_CorrectTimes(exception.GetType(), ref actualAttempts, () =>
                    ElementActionRetrier.DoWithRetry(() => {
                        Logger.Info($"current attempt is {actualAttempts++}");
                        throw exception;
                    }));
        }

        [Test]
        public void Retrier_ShouldWork_CorrectTimes_WithReturnValue([ValueSource(nameof(handledExceptions))] Exception exception)
        {
            var actualAttempts = 0;
            Retrier_ShouldWork_CorrectTimes(exception.GetType(), ref actualAttempts, () =>
                    ElementActionRetrier.DoWithRetry(() => {
                        Logger.Info($"current attempt is {actualAttempts++}");
                        throw exception;
#pragma warning disable CS0162 // Unreachable code detected
                        return string.Empty;
#pragma warning restore CS0162 // Unreachable code detected
                    }));
        }

        [Test]
        public void Retrier_Should_ReturnValue()
        {
            var returnValue = string.Empty;
            Assert.That(ElementActionRetrier.DoWithRetry(() => returnValue), Is.EqualTo(returnValue), "Retrier should return value");
        }
    }
}
