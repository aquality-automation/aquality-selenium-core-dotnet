using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Waitings;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace Aquality.Selenium.Core.Elements
{
    public class ElementStateProvider : IElementStateProvider
    {
        private readonly By elementLocator;
        private readonly LogElementState logElementState;

        public ElementStateProvider(By elementLocator, IConditionalWait conditionalWait, IElementFinder elementFinder, LogElementState logElementState)
        {
            this.elementLocator = elementLocator;
            ConditionalWait = conditionalWait;
            ElementFinder = elementFinder;
            this.logElementState = logElementState;
        }

        private IConditionalWait ConditionalWait { get; }

        private IElementFinder ElementFinder { get; }

        public bool IsDisplayed => WaitForDisplayed(TimeSpan.Zero);

        public bool IsExist => WaitForExist(TimeSpan.Zero);

        public bool IsEnabled => WaitForEnabled(TimeSpan.Zero);

        public bool IsClickable => IsElementClickable(TimeSpan.Zero, true);

        public bool WaitForDisplayed(TimeSpan? timeout = null)
        {
            return DoAndLogWaitForState(() => IsAnyElementFound(timeout, ElementState.Displayed), "displayed");
        }

        public bool WaitForNotDisplayed(TimeSpan? timeout = null)
        {
            return DoAndLogWaitForState(() => ConditionalWait.WaitFor(() => !IsDisplayed, timeout), "not.displayed");
        }

        public bool WaitForExist(TimeSpan? timeout = null)
        {
            return DoAndLogWaitForState(() => IsAnyElementFound(timeout, ElementState.ExistsInAnyState), "exist");
        }

        public bool WaitForNotExist(TimeSpan? timeout = null)
        {
            return DoAndLogWaitForState(() => ConditionalWait.WaitFor(() => !IsExist, timeout), "not.exist");
        }

        private bool IsAnyElementFound(TimeSpan? timeout, ElementState state)
        {
            return ElementFinder.FindElements(elementLocator, state, timeout).Any();
        }

        public bool WaitForEnabled(TimeSpan? timeout = null)
        {
            return DoAndLogWaitForState(() => IsElementInDesiredState(element => IsElementEnabled(element), "ENABLED", timeout), "enabled");
        }

        public bool WaitForNotEnabled(TimeSpan? timeout = null)
        {
            return DoAndLogWaitForState(() => IsElementInDesiredState(element => !IsElementEnabled(element), "NOT ENABLED", timeout), "not.enabled");
        }

        protected virtual bool IsElementEnabled(IWebElement element)
        {
            return element.Enabled;
        }

        private bool IsElementInDesiredState(Func<IWebElement, bool> elementStateCondition, string state, TimeSpan? timeout)
        {
            var desiredState = new DesiredState(elementStateCondition, state)
            {
                IsCatchingTimeoutException = true,
                IsThrowingNoSuchElementException = true
            };
            return IsElementInDesiredCondition(timeout, desiredState);
        }

        public void WaitForClickable(TimeSpan? timeout = null)
        {
            var conditionKey = "loc.el.state.clickable";
            try
            {
                logElementState("loc.wait.for.state", conditionKey);
                IsElementClickable(timeout, false);
            }
            catch
            {
                logElementState("loc.wait.for.state.failed", conditionKey);
                throw;
            }
        }

        private bool IsElementClickable(TimeSpan? timeout, bool catchTimeoutException)
        {
            var desiredState = new DesiredState(element => element.Displayed && element.Enabled, "CLICKABLE")
            {
                IsCatchingTimeoutException = catchTimeoutException
            };
            return IsElementInDesiredCondition(timeout, desiredState);
        }

        private bool IsElementInDesiredCondition(TimeSpan? timeout, DesiredState elementStateCondition)
        {
            return ElementFinder.FindElements(elementLocator, elementStateCondition, timeout).Any();
        }
        
        private bool DoAndLogWaitForState(Func<bool> waitingAction, string conditionKeyPart, TimeSpan? timeout = null)
        {
            if (TimeSpan.Zero == timeout)
            {
                return waitingAction();
            }

            var conditionKey = $"loc.el.state.{conditionKeyPart}";
            logElementState("loc.wait.for.state", conditionKey);
            var result = waitingAction();
            if (!result)
            {
                logElementState("loc.wait.for.state.failed", conditionKey);
            }

            return result;
        }
    }
}
