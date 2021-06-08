using Aquality.Selenium.Core.Elements.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;

namespace Aquality.Selenium.Core.Elements
{
    public class ElementCacheHandler : IElementCacheHandler
    {
        private readonly By locator;
        private readonly string name;
        private readonly ElementState state;
        private readonly IElementFinder elementFinder;

        private RemoteWebElement remoteElement;

        public ElementCacheHandler(By locator, string name, ElementState state, IElementFinder finder)
        {
            this.locator = locator;
            this.name = name;
            this.state = state;
            elementFinder = finder;
        }

        public bool IsStale => remoteElement != null && IsRefreshNeeded();

        public bool IsRefreshNeeded(ElementState? customState = null)
        {
            if (remoteElement == null)
            {
                return true;
            }
            try
            {
                var isDisplayed = remoteElement.Displayed;
                // refresh is needed only if the property is not match to expected element state
                return (customState ?? state) == ElementState.Displayed && !isDisplayed;
            }
            catch
            {
                // refresh is needed if the property is not available
                return true;
            }
        }

        public RemoteWebElement GetElement(TimeSpan? timeout = null, ElementState? customState = null)
        {

            if (IsRefreshNeeded(customState))
            {
                remoteElement = (RemoteWebElement)elementFinder.FindElement(locator, customState ?? state, timeout, name);
            }

            return remoteElement;
        }
    }
}
