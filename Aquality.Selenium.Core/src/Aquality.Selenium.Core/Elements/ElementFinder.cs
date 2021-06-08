using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Waitings;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Aquality.Selenium.Core.Elements
{
    /// <summary>
    /// Implementation of <see cref="IElementFinder"/>
    /// </summary>
    public class ElementFinder : IElementFinder
    {
        public ElementFinder(ILocalizedLogger logger, IConditionalWait conditionalWait)
        {
            Logger = logger;
            ConditionalWait = conditionalWait;
        }

        private ILocalizedLogger Logger { get; }

        private IConditionalWait ConditionalWait { get; }

        public virtual IWebElement FindElement(By locator, ElementState state = ElementState.ExistsInAnyState, TimeSpan? timeout = null, string name = null)
        {
            var desiredState = ResolveState(state);
            return FindElement(locator, desiredState.ElementStateCondition, desiredState.StateName, timeout, name);
        }

        public virtual IWebElement FindElement(By locator, Func<IWebElement, bool> elementStateCondition, TimeSpan? timeout = null, string name = null)
        {
            return FindElement(locator, elementStateCondition, "desired", timeout, name);
        }

        public virtual IWebElement FindElement(By locator, Func<IWebElement, bool> elementStateCondition, string stateName, TimeSpan? timeout = null, string name = null)
        {
            var desiredState = new DesiredState(elementStateCondition, stateName)
            {
                IsCatchingTimeoutException = false,
                IsThrowingNoSuchElementException = true
            };
            return FindElements(locator, desiredState, timeout, name).First();
        }

        public virtual ReadOnlyCollection<IWebElement> FindElements(By locator, ElementState state = ElementState.ExistsInAnyState, TimeSpan? timeout = null, string name = null)
        {
            var elementStateCondition = ResolveState(state);
            elementStateCondition.IsCatchingTimeoutException = true;
            return FindElements(locator, elementStateCondition, timeout, name);
        }

        public virtual ReadOnlyCollection<IWebElement> FindElements(By locator, Func<IWebElement, bool> elementStateCondition, TimeSpan? timeout = null, string name = null)
        {
            var desiredState = new DesiredState(elementStateCondition, "desired")
            {
                IsCatchingTimeoutException = true,
            };
            return FindElements(locator, desiredState, timeout, name);
        }

        public virtual ReadOnlyCollection<IWebElement> FindElements(By locator, DesiredState desiredState, TimeSpan? timeout = null, string name = null)
        {
            var foundElements = new List<IWebElement>();
            var resultElements = new List<IWebElement>();
            try
            {
                ConditionalWait.WaitFor(driver =>
                {
                    foundElements = driver.FindElements(locator).ToList();
                    resultElements = foundElements.Where(desiredState.ElementStateCondition).ToList();
                    return resultElements.Any();
                }, timeout);
            }
            catch (WebDriverTimeoutException ex)
            {
                HandleTimeoutException(ex, desiredState, locator, foundElements, name);
            }
            return resultElements.AsReadOnly();
        }

        protected virtual void HandleTimeoutException(WebDriverTimeoutException ex, DesiredState desiredState, By locator, List<IWebElement> foundElements, string name = null)
        {
            var message = string.IsNullOrEmpty(name) 
                ? $"No elements with locator '{locator}' were found in {desiredState.StateName} state"
                : $"Element [{name}] was not found by locator '{locator}' in {desiredState.StateName} state";
            if (desiredState.IsCatchingTimeoutException)
            {
                if (!foundElements.Any())
                {
                    if (desiredState.IsThrowingNoSuchElementException)
                    {
                        throw new NoSuchElementException(message);
                    }
                    Logger.Debug("loc.no.elements.found.in.state", null, locator.ToString(), desiredState.StateName);
                }
                else
                {
                    Logger.Debug("loc.elements.were.found.but.not.in.state", null, locator.ToString(), desiredState.StateName);
                }
            }
            else
            {
                if (desiredState.IsThrowingNoSuchElementException && !foundElements.Any())
                {
                    throw new NoSuchElementException($"{message}: {ex.Message}");
                }
                throw new WebDriverTimeoutException($"{ex.Message}: {message}");
            }
        }

        protected virtual DesiredState ResolveState(ElementState state)
        {
            Func<IWebElement, bool> elementStateCondition;
            switch (state)
            {
                case ElementState.Displayed:
                    elementStateCondition = element => element.Displayed;
                    break;
                case ElementState.ExistsInAnyState:
                    elementStateCondition = element => true;
                    break;
                default:
                    throw new InvalidOperationException($"{state} state is not recognized");
            }

            return new DesiredState(elementStateCondition, state.ToString());
        }
    }
}
