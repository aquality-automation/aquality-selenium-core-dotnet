﻿using System;
using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Utilities;
using Aquality.Selenium.Core.Waitings;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Aquality.Selenium.Core.Elements
{
    /// <summary>
    /// Defines base class for any UI element.
    /// </summary>
    public abstract class Element : IElement
    {
        private readonly ElementState elementState;
        private IElementCacheHandler elementCacheHandler;

        protected Element(By locator, string name, ElementState state)
        {
            Locator = locator;
            Name = name;
            elementState = state;
        }

        public By Locator { get; }

        public string Name { get; }

        public virtual IElementStateProvider State => CacheConfiguration.IsEnabled 
            ? (IElementStateProvider) new CachedElementStateProvider(Locator, ConditionalWait, Cache)
            : new ElementStateProvider(Locator, ConditionalWait, Finder);

        protected virtual IElementCacheHandler Cache
        {
            get
            {
                if (elementCacheHandler == null)
                {
                    elementCacheHandler = new ElementCacheHandler(Locator, elementState, Finder);
                }

                return elementCacheHandler;
            }
        }

        protected abstract IElementActionRetrier ActionRetrier { get; }

        protected abstract IApplication Application { get; }

        protected abstract IElementCacheConfiguration CacheConfiguration { get; }

        protected abstract ConditionalWait ConditionalWait { get; }

        protected abstract string ElementType { get; }

        protected abstract IElementFactory Factory { get; }

        protected abstract IElementFinder Finder { get; }

        protected abstract ILocalizedLogger LocalizedLogger { get; }

        protected virtual Logger Logger => Logger.Instance;

        public void Click()
        {
            LogElementAction("loc.clicking");
            DoWithRetry(() => GetElement().Click());
        }

        public T FindChildElement<T>(By childLocator, string name = null, ElementSupplier<T> supplier = null, ElementState state = ElementState.Displayed) where T : IElement
        {
            return Factory.FindChildElement(this, childLocator, name, supplier, state);
        }

        public string GetAttribute(string attr)
        {
            LogElementAction("loc.el.getattr", attr);
            return DoWithRetry(() => GetElement().GetAttribute(attr));
        }

        public virtual RemoteWebElement GetElement(TimeSpan? timeout = null)
        {
            try
            {
                return CacheConfiguration.IsEnabled 
                    ? Cache.GetElement(timeout)
                    : (RemoteWebElement) Finder.FindElement(Locator, elementState, timeout);
            }
            catch (NoSuchElementException ex)
            {
                LogPageSource(ex);
                throw;
            }
        }

        private void LogPageSource(WebDriverException exception)
        {
            try
            {
                Logger.Debug($"Page source:{Environment.NewLine}{Application.Driver.PageSource}", exception);
            }
            catch (WebDriverException e)
            {
                Logger.Error(exception.Message);
                Logger.Fatal("An exception occured while tried to save the page source", e);
            }
        }

        public string Text
        {
            get
            {
                LogElementAction("loc.get.text");
                return DoWithRetry(() => GetElement().Text);
            }
        }

        public void SendKeys(string key)
        {
            LogElementAction("loc.text.sending.keys", key);
            DoWithRetry(() => GetElement().SendKeys(key));
        }

        protected virtual void DoWithRetry(Action action)
        {
            ActionRetrier.DoWithRetry(action);
        }

        protected virtual T DoWithRetry<T>(Func<T> function)
        {
            return ActionRetrier.DoWithRetry(function);
        }

        protected virtual void LogElementAction(string messageKey, params object[] args)
        {
            LocalizedLogger.InfoElementAction(ElementType, Name, messageKey, args);
        }
    }
}
