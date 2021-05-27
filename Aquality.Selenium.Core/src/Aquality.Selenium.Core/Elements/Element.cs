using System;
using System.Collections.Generic;
using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Utilities;
using Aquality.Selenium.Core.Visualization;
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
        internal readonly ElementState elementState;
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
            ? (IElementStateProvider) new CachedElementStateProvider(Locator, ConditionalWait, Cache, LogElementState)
            : new ElementStateProvider(Locator, ConditionalWait, Finder, LogElementState);

        public virtual IVisualStateProvider Visual 
            => new VisualStateProvider(ImageComparator, ActionRetrier, () => GetElement(), LogVisualState);

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

        protected abstract IConditionalWait ConditionalWait { get; }

        protected abstract string ElementType { get; }

        protected abstract IElementFactory Factory { get; }

        protected abstract IElementFinder Finder { get; }

        protected abstract IImageComparator ImageComparator { get; }

        protected abstract ILocalizedLogger LocalizedLogger { get; }

        protected abstract ILocalizationManager LocalizationManager { get; }

        protected virtual ILoggerConfiguration LoggerConfiguration => LocalizedLogger.Configuration;

        protected virtual Logger Logger => Logger.Instance;

        protected virtual LogElementState LogElementState =>
            (string messageKey, string stateKey)
            => LocalizedLogger.InfoElementAction(ElementType, Name, messageKey, LocalizationManager.GetLocalizedMessage(stateKey));

        protected virtual LogVisualState LogVisualState =>
            (string messageKey, object[] values) =>
            {
                if (values == null || values.Length == 0)
                {
                    LogElementAction(messageKey);
                }
                else
                {
                    LogElementAction(messageKey, values);
                }
            };

        public void Click()
        {
            LogElementAction("loc.clicking");
            DoWithRetry(() => GetElement().Click());
        }

        public T FindChildElement<T>(By childLocator, string name = null, ElementSupplier<T> supplier = null, ElementState state = ElementState.Displayed) where T : IElement
        {
            return Factory.FindChildElement(this, childLocator, name, supplier, state);
        }

        public IList<T> FindChildElements<T>(By childLocator, string name = null, ElementSupplier<T> supplier = null, ElementsCount expectedCount = ElementsCount.Any, ElementState state = ElementState.Displayed) where T : IElement
        {
            return Factory.FindChildElements(this, childLocator, name, supplier, expectedCount, state);
        }

        public string GetAttribute(string attr)
        {
            LogElementAction("loc.el.getattr", attr);
            var value = DoWithRetry(() => GetElement().GetAttribute(attr));
            LogElementAction("loc.el.attr.value", attr, value);

            return value;
        }

        public virtual RemoteWebElement GetElement(TimeSpan? timeout = null)
        {
            try
            {
                return CacheConfiguration.IsEnabled
                    ? Cache.GetElement(timeout)
                    : (RemoteWebElement) Finder.FindElement(Locator, elementState, timeout);
            }
            catch (NoSuchElementException ex) when (LoggerConfiguration.LogPageSource)
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
                Logger.Fatal("An exception occurred while tried to save the page source", e);
            }
        }

        public string Text
        {
            get
            {
                LogElementAction("loc.get.text");
                var value = DoWithRetry(() => GetElement().Text);
                LogElementAction("loc.text.value", value);

                return value;
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
