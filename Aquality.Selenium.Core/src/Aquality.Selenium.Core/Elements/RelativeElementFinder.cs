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
    /// Implementation of <see cref="IElementFinder"/> for a relative <see cref="ISearchContext"/> supplier
    /// </summary>
    public class RelativeElementFinder : ElementFinder
    {
        public RelativeElementFinder(ILocalizedLogger logger, ConditionalWait conditionalWait, Func<ISearchContext> searchContextSupplier) 
            : base(logger, conditionalWait)
        {
            ConditionalWait = conditionalWait;
            SearchContextSupplier = searchContextSupplier;
        }

        private ConditionalWait ConditionalWait { get; }

        private Func<ISearchContext> SearchContextSupplier { get; }

        public override ReadOnlyCollection<IWebElement> FindElements(By locator, DesiredState desiredState, TimeSpan? timeout = null)
        {
            var foundElements = new List<IWebElement>();
            var resultElements = new List<IWebElement>();
            try
            {
                ConditionalWait.WaitFor(() =>
                {
                    foundElements = SearchContextSupplier().FindElements(locator).ToList();
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
    }
}
