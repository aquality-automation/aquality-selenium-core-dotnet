using Aquality.Selenium.Core.Elements.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;

namespace Aquality.Selenium.Core.Elements
{
    public class ElementCacheHandler : IElementCacheHandler
    {
        private readonly By locator;
        private readonly ElementState state;
        private readonly IElementFinder elementFinder;

        private RemoteWebElement remoteElement;

        public ElementCacheHandler(By locator, ElementState state, IElementFinder finder)
        {
            this.locator = locator;
            this.state = state;
            elementFinder = finder;
        }

        public bool IsStale => remoteElement != null && IsRefreshNeeded;

        public bool IsRefreshNeeded
        {
            get
            {
                if (remoteElement == null)
                {
                    return true;
                }
                try
                {
                    var isDisplayed = remoteElement.Displayed;
                    // no refresh needed if the property is available
                    return false;
                }
                catch
                {
                    return true;
                }
            }
        }

        public RemoteWebElement GetElement(TimeSpan? timeout = null)
        {

            if (IsRefreshNeeded)
            {
                remoteElement = (RemoteWebElement)elementFinder.FindElement(locator, state, timeout);
            }

            return remoteElement;
        }
    }
}
