using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Tests.Applications.Browser;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace Aquality.Selenium.Core.Tests.Utilities
{
    public abstract class RetrierTests : TestWithBrowser
    {
        protected const int ACCURACY = 100;

        protected Logger Logger => AqualityServices.ServiceProvider.GetRequiredService<Logger>();

        protected IRetryConfiguration RertyConfiguration => AqualityServices.ServiceProvider.GetRequiredService<IRetryConfiguration>();
        
        protected int PollingInterval => RertyConfiguration.PollingInterval.Milliseconds;

        protected int RetriesCount => RertyConfiguration.Number;

        protected void Retrier_ShouldWork_OnceIfMethodSucceeded(Action action)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            action();
            watch.Stop();
            var duration = watch.ElapsedMilliseconds;

            Assert.IsTrue(duration < PollingInterval,
                $"Duration '{duration}' should be less that pollingInterval '{PollingInterval}'");
        }

        protected void Retrier_ShouldWait_PollingIntervalBetweenMethodsCall(Action action)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            action();
            watch.Stop();
            var duration = watch.ElapsedMilliseconds;
            var doubledAccuracyPollingInterval = 2 * PollingInterval + ACCURACY;

            Assert.IsTrue(PollingInterval <= duration && duration <= doubledAccuracyPollingInterval, 
                $"Duration '{duration}' should be more than '{PollingInterval}' and less than '{doubledAccuracyPollingInterval}'");
        }

        protected void Retrier_ShouldWork_CorrectTimes(Type handledException, ref int actualAttempts, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Assert.IsTrue(handledException.IsAssignableFrom(e.GetType()));
            }
            Assert.AreEqual(actualAttempts, RetriesCount + 1, "actual attempts count is not match to expected");
        }
    }
}
