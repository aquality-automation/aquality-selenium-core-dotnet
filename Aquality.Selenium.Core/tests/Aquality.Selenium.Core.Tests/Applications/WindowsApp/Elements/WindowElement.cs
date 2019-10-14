using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Utilities;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements
{
    public abstract class WindowElement : Element
    {
        protected WindowElement(By locator, string name) : base(locator, name, ElementState.Displayed)
        {
        }

        protected override ElementActionRetrier ActionRetrier => ApplicationManager.ServiceProvider.GetRequiredService<ElementActionRetrier>();

        protected override IApplication Application => ApplicationManager.Application;

        protected override ConditionalWait ConditionalWait => ApplicationManager.ServiceProvider.GetRequiredService<ConditionalWait>();

        protected override IElementFactory Factory => ApplicationManager.ServiceProvider.GetRequiredService<IElementFactory>();

        protected override IElementFinder Finder => ApplicationManager.ServiceProvider.GetRequiredService<IElementFinder>();

        protected override ILocalizationLogger LocalizationLogger => ApplicationManager.ServiceProvider.GetRequiredService<ILocalizationLogger>();
    }
}
