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
        public ElementFinder(ILocalizedLogger logger, ConditionalWait conditionalWait)
        {
            Logger = logger;
            ConditionalWait = conditionalWait;
        }

        private ILocalizedLogger Logger { get; }

        private ConditionalWait ConditionalWait { get; }

        public IWebElement FindElement(By locator, ElementState state = ElementState.ExistsInAnyState, TimeSpan? timeout = null)
        {
            var desiredState = ResolveState(state);
            return FindElement(locator, desiredState.ElementStateCondition, desiredState.StateName, timeout);
        }

        public IWebElement FindElement(By locator, Func<IWebElement, bool> elementStateCondition, TimeSpan? timeout = null)
        {
            return FindElement(locator, elementStateCondition, "desired", timeout);
        }

        private IWebElement FindElement(By locator, Func<IWebElement, bool> elementStateCondition, string stateName, TimeSpan? timeout = null)
        {
            var desiredState = new DesiredState(elementStateCondition, stateName)
            {
                IsCatchingTimeoutException = false,
                IsThrowingNoSuchElementException = true
            };
            return FindElements(locator, desiredState, timeout).First();
        }

        public ReadOnlyCollection<IWebElement> FindElements(By locator, ElementState state = ElementState.ExistsInAnyState, TimeSpan? timeout = null)
        {
            var elementStateCondition = ResolveState(state);
            elementStateCondition.IsCatchingTimeoutException = true;
            return FindElements(locator, elementStateCondition, timeout);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By locator, Func<IWebElement, bool> elementStateCondition, TimeSpan? timeout = null)
        {
            var desiredState = new DesiredState(elementStateCondition, "desired")
            {
                IsCatchingTimeoutException = true
            };
            return FindElements(locator, desiredState, timeout);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By locator, DesiredState desiredState, TimeSpan? timeout = null)
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
                HandleTimeoutException(ex, desiredState, locator, foundElements);
            }
            return resultElements.AsReadOnly();
        }

        private void HandleTimeoutException(WebDriverTimeoutException ex, DesiredState desiredState, By locator, List<IWebElement> foundElements)
        {
            var message = $"No elements with locator '{locator.ToString()}' were found in {desiredState.StateName} state";
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
                var combinedMessage = $"{ex.Message}: {message}";
                if (desiredState.IsThrowingNoSuchElementException && !foundElements.Any())
                {
                    throw new NoSuchElementException(combinedMessage);
                }
                throw new WebDriverTimeoutException(combinedMessage);
            }
        }

        private DesiredState ResolveState(ElementState state)
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
