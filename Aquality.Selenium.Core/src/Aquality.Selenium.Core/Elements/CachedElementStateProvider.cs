using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Waitings;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aquality.Selenium.Core.Elements
{
    public class CachedElementStateProvider : IElementStateProvider
    {
        private readonly IElementCacheHandler elementCacheHandler;
        private readonly ConditionalWait conditionalWait;
        private readonly By locator;

        public CachedElementStateProvider(By locator, ConditionalWait conditionalWait, IElementCacheHandler elementCacheHandler)
        {
            this.elementCacheHandler = elementCacheHandler;
            this.conditionalWait = conditionalWait;
            this.locator = locator;
        }

        protected virtual bool TryInvokeFunction(Func<IWebElement, bool> func, IList<Type> exceptionsToHandle = null)
        {
            try
            {
                return func(elementCacheHandler.GetElement(TimeSpan.Zero));
            }
            catch (Exception e)
            {
                if (exceptionsToHandle != null && exceptionsToHandle.Any(type => type.IsAssignableFrom(e.GetType())))
                {
                    return false;
                }
                throw;
            }
        }

        public virtual bool IsDisplayed => !elementCacheHandler.IsStale 
            && TryInvokeFunction(element => element.Displayed, new List<Type> { typeof(StaleElementReferenceException), typeof(NoSuchElementException) });

        public virtual bool IsExist => !elementCacheHandler.IsStale
            && TryInvokeFunction(element => true, new List<Type> { typeof(NoSuchElementException) });

        public virtual bool IsClickable => TryInvokeFunction(element => element.Displayed && element.Enabled);

        public virtual bool IsEnabled => TryInvokeFunction(element => element.Enabled);

        public virtual void WaitForClickable(TimeSpan? timeout = null)
        {
            var errorMessage = $"Element {locator} has not become clickable after timeout.";
            conditionalWait.WaitForTrue(() => IsClickable, timeout, message: errorMessage);
        }

        public virtual bool WaitForDisplayed(TimeSpan? timeout = null)
        {
            return WaitForConditionWithLogging(() => TryInvokeFunction(element => element.Displayed), "displayed", timeout);
        }

        public virtual bool WaitForEnabled(TimeSpan? timeout = null)
        {
            return WaitForConditionWithLogging(() => IsEnabled, "enabled", timeout);
        }

        public virtual bool WaitForExist(TimeSpan? timeout = null)
        {
            return WaitForConditionWithLogging(() => TryInvokeFunction(element => true), "exist", timeout);
        }

        public virtual bool WaitForNotDisplayed(TimeSpan? timeout = null)
        {
            return WaitForConditionWithLogging(() => !IsDisplayed, "invisible or absent", timeout);
        }

        public virtual bool WaitForNotEnabled(TimeSpan? timeout = null)
        {
            return WaitForConditionWithLogging(() => !IsEnabled, "disabled", timeout);
        }

        public virtual bool WaitForNotExist(TimeSpan? timeout = null)
        {
            return WaitForConditionWithLogging(() => !IsExist, "absent", timeout);
        }

        protected virtual bool WaitForConditionWithLogging(Func<bool> condition, string conditionName, TimeSpan? timeout)
        {
            var result = conditionalWait.WaitFor(condition, timeout);
            if (!result)
            {
                var timeoutString = timeout == null ? string.Empty : $"of {timeout.Value.TotalSeconds} seconds";
                Logger.Instance.Warn($"Element {locator} has not become {conditionName} after timeout {timeoutString}");
            }

            return result;
        }
    }
}
