using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Utilities;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public abstract class WebElement : Element
    {
        protected WebElement(By locator, string name, ElementState state) : base(locator, name, state)
        {
        }

        protected override ElementActionRetrier ActionRetrier => AqualityServices.ServiceProvider.GetRequiredService<ElementActionRetrier>();

        protected override IApplication Application => AqualityServices.Application;

        protected override ConditionalWait ConditionalWait => AqualityServices.ServiceProvider.GetRequiredService<ConditionalWait>();

        protected override IElementFactory Factory => AqualityServices.ServiceProvider.GetRequiredService<IElementFactory>();

        protected override IElementFinder Finder => AqualityServices.ServiceProvider.GetRequiredService<IElementFinder>();

        protected override ILocalizedLogger LocalizedLogger => AqualityServices.ServiceProvider.GetRequiredService<ILocalizedLogger>();
    }
}
