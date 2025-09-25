using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class ConditionalWaitTests : TestWithBrowser
    {
        private static readonly Uri WikiURL = new("https://wikipedia.org");
        private static readonly TimeSpan LittleTimeout = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan PollingInterval = AqualityServices.ServiceProvider.GetRequiredService<ITimeoutConfiguration>().PollingInterval;

        private static readonly Action<Func<bool>, IList<Type>>[] WaitWithHandledException
            =
            [
                (condition, handledExceptions) => ConditionalWait.WaitFor(condition, timeout: LittleTimeout, exceptionsToIgnore: handledExceptions),
                (condition, handledExceptions) => ConditionalWait.WaitFor(driver => condition(), timeout: LittleTimeout, exceptionsToIgnore: handledExceptions),
                (condition, handledExceptions) => ConditionalWait.WaitForTrue(condition, timeout: LittleTimeout, exceptionsToIgnore: handledExceptions)
            ];

        private static readonly Func<Func<bool>, IList<Type>, Task>[] WaitWithHandledExceptionAsync
            =
            [
                (condition, handledExceptions) => ConditionalWait.WaitForAsync(condition, timeout: LittleTimeout, exceptionsToIgnore: handledExceptions),
                (condition, handledExceptions) => ConditionalWait.WaitForTrueAsync(condition, timeout: LittleTimeout, exceptionsToIgnore: handledExceptions)
            ];

        private static IConditionalWait ConditionalWait => AqualityServices.ServiceProvider.GetRequiredService<IConditionalWait>();

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_WithDriver()
        {
            Assert.DoesNotThrow(() => ConditionalWait.WaitFor(driver =>
            {
                GoToUrl(WikiURL, driver);
                return driver.FindElements(By.XPath("//*")).Count > 0;
            }));
        }

        [Test]
        public void Should_NotThrow_OnWait_WithHandledException([ValueSource(nameof(WaitWithHandledException))] Action<Func<bool>, IList<Type>> action)
        {
            var i = 0;
            var exception = new AssertionException("Failure during conditional wait in handled exception");
            Assert.DoesNotThrow(() => action(() =>
            {
                return ++i == 2 ? true : throw exception;
            }, [exception.GetType()]
            ), nameof(Should_NotThrow_OnWait_WithHandledException));
        }

        [Test]
        public void Should_NotThrowAsync_OnWait_WithHandledException([ValueSource(nameof(WaitWithHandledExceptionAsync))] Func<Func<bool>, IList<Type>, Task> action)
        {
            var i = 0;
            var exception = new AssertionException("Failure during conditional wait in handled exception");
            Assert.DoesNotThrowAsync(() => action(() =>
            {
                return ++i == 2 ? true : throw exception;
            }, [exception.GetType()]
            ), nameof(Should_NotThrow_OnWait_WithHandledException));
        }

        [Test]
        public void Should_ThrowAsync_OnWait_WithUnhandledException([ValueSource(nameof(WaitWithHandledExceptionAsync))] Func<Func<bool>, IList<Type>, Task> action)
        {
            var i = 0;
            var exception = new AssertionException("Failure during conditional wait in handled exception");
            Assert.ThrowsAsync<AssertionException>(() => action(() =>
            {
                return ++i == 2 ? true : throw exception;
            }, [typeof(InvalidOperationException)]
            ), nameof(Should_ThrowAsync_OnWait_WithUnhandledException));
        }

        [Test]
        public void Should_Throw_OnWait_WithUnhandledException([ValueSource(nameof(WaitWithHandledException))] Action<Func<bool>, IList<Type>> action)
        {
            var i = 0;
            var exception = new AssertionException("Failure during conditional wait in handled exception");
            Assert.Throws<AssertionException>(() => action(() =>
            {
                return ++i == 2 ? true : throw exception;
            }, [typeof(InvalidOperationException)]
            ), nameof(Should_Throw_OnWait_WithUnhandledException));
        }

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_WithElementFinder()
        {
            bool elementFinderCondition() => ServiceProvider.GetRequiredService<IElementFinder>()
                .FindElements(By.XPath("//*[contains(., 'wikipedia')]"), timeout: LittleTimeout).Count > 0;
            Assert.That(elementFinderCondition(), Is.False);
            Assert.DoesNotThrow(() => ConditionalWait.WaitFor(driver =>
            {
                GoToUrl(WikiURL, driver);
                return elementFinderCondition();
            }));
        }

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_ForAsyncWaiting()
        {
            bool result = true, returnedResult = true;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Assert.DoesNotThrow(() => returnedResult = ConditionalWait.WaitForAsync(() => result = false, LittleTimeout).Result);
            stopWatch.Stop();
            Assert.That(result, Is.False, $"{nameof(ConditionalWait.WaitForAsync)} should work at least once");
            Assert.That(returnedResult, Is.False, $"{nameof(ConditionalWait.WaitForAsync)} should return valid result");
            Assert.That(stopWatch.Elapsed.TotalSeconds, Is.EqualTo(LittleTimeout.TotalSeconds).Within(PollingInterval.TotalSeconds * 2), 
                $"{nameof(ConditionalWait.WaitForAsync)} should wait correct time");
        }

        [Test, Retry(RetriesNumber)]
        public void Should_BePossibleTo_UseConditionalWait_ForAsyncWaitingForTrue()
        {
            var bigTimeout = AqualityServices.ServiceProvider.GetRequiredService<ITimeoutConfiguration>().Condition;
            bool result = true;
            Task awaitableResult = null;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Assert.DoesNotThrow(() => awaitableResult = ConditionalWait.WaitForTrueAsync(() => result = false, LittleTimeout),
                        $"{nameof(ConditionalWait.WaitForTrueAsync)} should not fail after the calling");
            Assume.That(awaitableResult, Is.Not.Null);
            Assert.ThrowsAsync<TimeoutException>(async () => await awaitableResult, $"{nameof(ConditionalWait.WaitForTrueAsync)} should throw when awaited");
            Assert.That(result, Is.False, $"{nameof(ConditionalWait.WaitForTrueAsync)} should work");
            stopWatch.Stop();
            Assert.That(stopWatch.Elapsed.TotalSeconds, Is.EqualTo(LittleTimeout.TotalSeconds).Within(PollingInterval.TotalSeconds * 2), 
                $"{nameof(ConditionalWait.WaitForTrueAsync)} should wait correct time");
        }
    }
}
