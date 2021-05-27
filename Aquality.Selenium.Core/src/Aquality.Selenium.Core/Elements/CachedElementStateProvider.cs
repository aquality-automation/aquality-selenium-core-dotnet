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
        private readonly LogElementState logElementState;
        private readonly IConditionalWait conditionalWait;
        private readonly By locator;

        public CachedElementStateProvider(By locator, IConditionalWait conditionalWait, IElementCacheHandler elementCacheHandler, LogElementState logElementState)
        {
            this.elementCacheHandler = elementCacheHandler;
            this.logElementState = logElementState;
            this.conditionalWait = conditionalWait;
            this.locator = locator;
        }

        protected virtual IList<Type> HandledExceptions => new List<Type> { typeof(StaleElementReferenceException), typeof(NoSuchElementException) };

        protected virtual bool TryInvokeFunction(Func<IWebElement, bool> func, IList<Type> exceptionsToHandle = null)
        {
            var handledExceptions = exceptionsToHandle ?? HandledExceptions;
            try
            {
                return func(elementCacheHandler.GetElement(TimeSpan.Zero, ElementState.ExistsInAnyState));
            }
            catch (Exception e)
            {
                if (handledExceptions.Any(type => type.IsAssignableFrom(e.GetType())))
                {
                    return false;
                }
                throw;
            }
        }

        public virtual bool IsDisplayed => !elementCacheHandler.IsStale && TryInvokeFunction(element => element.Displayed);

        public virtual bool IsExist => !elementCacheHandler.IsStale && TryInvokeFunction(element => true);

        public virtual bool IsClickable => TryInvokeFunction(element => element.Displayed && element.Enabled);

        public virtual bool IsEnabled => TryInvokeFunction(element => element.Enabled, new[] { typeof(StaleElementReferenceException) });

        public virtual void WaitForClickable(TimeSpan? timeout = null)
        {
            var errorMessage = $"Element {locator} has not become clickable after timeout.";
            var conditionKey = "loc.el.state.clickable";
            try
            {
                logElementState("loc.wait.for.state", conditionKey);
                conditionalWait.WaitForTrue(() => IsClickable, timeout, message: errorMessage);
            }
            catch (TimeoutException e)
            {
                logElementState("loc.wait.for.state.failed", conditionKey);
                throw new WebDriverTimeoutException(e.Message, e);
            }
            
        }

        public virtual bool WaitForDisplayed(TimeSpan? timeout = null)
        {
            return WaitForCondition(() => TryInvokeFunction(element => element.Displayed), "displayed", timeout);
        }

        public virtual bool WaitForEnabled(TimeSpan? timeout = null)
        {
            return WaitForCondition(() => IsEnabled, "enabled", timeout);
        }

        public virtual bool WaitForExist(TimeSpan? timeout = null)
        {
            return WaitForCondition(() => TryInvokeFunction(element => true), "exist", timeout);
        }

        public virtual bool WaitForNotDisplayed(TimeSpan? timeout = null)
        {
            return WaitForCondition(() => !IsDisplayed, "not.displayed", timeout);
        }

        public virtual bool WaitForNotEnabled(TimeSpan? timeout = null)
        {
            return WaitForCondition(() => !IsEnabled, "not.enabled", timeout);
        }

        public virtual bool WaitForNotExist(TimeSpan? timeout = null)
        {
            return WaitForCondition(() => !IsExist, "not.exist", timeout);
        }

        protected virtual bool WaitForCondition(Func<bool> condition, string conditionKeyPart, TimeSpan? timeout)
        {
            var conditionKey = $"loc.el.state.{conditionKeyPart}";
            logElementState("loc.wait.for.state", conditionKey);
            var result = conditionalWait.WaitFor(condition, timeout);
            if (!result)
            {
                logElementState("loc.wait.for.state.failed", conditionKey);
            }

            return result;
        }
    }
}
