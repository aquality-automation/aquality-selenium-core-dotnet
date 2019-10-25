using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Utilities;
using Aquality.Selenium.Core.Waitings;
using OpenQA.Selenium;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements
{
    public abstract class WindowElement : Element
    {
        protected WindowElement(By locator, string name) : base(locator, name, ElementState.Displayed)
        {
        }

        protected override ElementActionRetrier ActionRetrier => Startup.GetRequiredService<ElementActionRetrier>();

        protected override IApplication Application => ApplicationManager.Application;

        protected override ConditionalWait ConditionalWait => Startup.GetRequiredService<ConditionalWait>();

        protected override IElementFactory Factory => Startup.GetRequiredService<IElementFactory>();

        protected override IElementFinder Finder => Startup.GetRequiredService<IElementFinder>();

        protected override ILocalizedLogger LocalizedLogger => Startup.GetRequiredService<ILocalizedLogger>();
    }
}
