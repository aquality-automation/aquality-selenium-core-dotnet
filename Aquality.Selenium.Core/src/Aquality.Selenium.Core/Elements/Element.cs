using System;
using Aquality.Selenium.Core.Applications;
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

        protected Element(By locator, string name, ElementState state)
        {
            Locator = locator;
            Name = name;
            elementState = state;
        }

        public By Locator { get; }

        public string Name { get; }

        public virtual IElementStateProvider State => new ElementStateProvider(Locator, ConditionalWait, Finder);

        protected abstract ElementActionRetrier ActionRetrier { get; }

        protected abstract IApplication Application { get; }

        protected abstract ConditionalWait ConditionalWait { get; }

        protected abstract string ElementType { get; }

        protected abstract ElementFactory Factory { get; }

        protected abstract ElementFinder Finder { get; }

        protected abstract LocalizationLogger LocalizationLogger { get; }

        protected virtual Logger Logger => Logger.Instance;

        public void Click()
        {
            LogElementAction("loc.clicking");
            DoWithRetry(() => GetElement().Click());
        }

        public T FindChildElement<T>(By childLocator, ElementSupplier<T> supplier = null, ElementState state = ElementState.Displayed) where T : IElement
        {
            throw new NotImplementedException();
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
                return (RemoteWebElement)Finder.FindElement(Locator, elementState, timeout);
            }
            catch (NoSuchElementException ex)
            {
                Logger.Debug($"Page source:{Environment.NewLine}{Application.Driver.PageSource}", ex);
                throw;
            }
        }

        public string GetText()
        {
            LogElementAction("loc.get.text");
            return DoWithRetry(() => GetElement().Text);
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
            LocalizationLogger.InfoElementAction(ElementType, Name, messageKey, args);
        }
    }
}
