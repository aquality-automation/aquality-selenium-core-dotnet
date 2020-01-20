using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class ConditionalWaitTests : TestWithBrowser
    {
        private static readonly Uri WikiURL = new Uri("https://wikipedia.org");
        private static readonly TimeSpan LittleTimeout = TimeSpan.FromSeconds(1);
        
        private static readonly Action<Func<bool>, IList<Type>>[] WaitWithHandledException
            = new Action<Func<bool>, IList<Type>>[]
            {
                (condition, handledExceptions) => ConditionalWait.WaitFor(condition, timeout: LittleTimeout, exceptionsToIgnore: handledExceptions),
                (condition, handledExceptions) => ConditionalWait.WaitFor(driver => condition(), timeout: LittleTimeout, exceptionsToIgnore: handledExceptions),
                (condition, handledExceptions) => ConditionalWait.WaitForTrue(condition, timeout: LittleTimeout, exceptionsToIgnore: handledExceptions)
            };

        private static IConditionalWait ConditionalWait => AqualityServices.ServiceProvider.GetRequiredService<IConditionalWait>();

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_WithDriver()
        {
            Assert.DoesNotThrow(() => ConditionalWait.WaitFor(driver =>
            {
                driver.Navigate().GoToUrl(WikiURL);
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
            }, new[] { exception.GetType() }
            ), nameof(Should_NotThrow_OnWait_WithHandledException));
        }

        [Test]
        public void Should_Throw_OnWait_WithUnhandledException([ValueSource(nameof(WaitWithHandledException))] Action<Func<bool>, IList<Type>> action)
        {
            var i = 0;
            var exception = new AssertionException("Failure during conditional wait in handled exception");
            Assert.Throws<AssertionException>(() => action(() =>
            {
                return ++i == 2 ? true : throw exception;
            }, new[] { typeof(InvalidOperationException) }
            ), nameof(Should_Throw_OnWait_WithUnhandledException));
        }

        [Test]
        public void Should_BePossibleTo_UseConditionalWait_WithElementFinder()
        {
            bool elementFinderCondition() => ServiceProvider.GetRequiredService<IElementFinder>()
                .FindElements(By.XPath("//*[contains(., 'wikipedia')]"), timeout: LittleTimeout).Count > 0;
            Assert.IsFalse(elementFinderCondition());
            Assert.DoesNotThrow(() => ConditionalWait.WaitFor(driver =>
            {
                driver.Navigate().GoToUrl(WikiURL);
                return elementFinderCondition();
            }));
        }
    }
}
