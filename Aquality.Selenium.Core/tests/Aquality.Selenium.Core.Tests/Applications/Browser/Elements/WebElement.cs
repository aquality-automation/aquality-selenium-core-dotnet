using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
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

        protected override IElementActionRetrier ActionRetrier => AqualityServices.ServiceProvider.GetRequiredService<IElementActionRetrier>();

        protected override IApplication Application => AqualityServices.Application;

        protected override IElementCacheConfiguration CacheConfiguration => AqualityServices.ServiceProvider.GetRequiredService<IElementCacheConfiguration>();

        protected override IConditionalWait ConditionalWait => AqualityServices.ServiceProvider.GetRequiredService<IConditionalWait>();

        protected override IElementFactory Factory => AqualityServices.ServiceProvider.GetRequiredService<IElementFactory>();

        protected override IElementFinder Finder => AqualityServices.ServiceProvider.GetRequiredService<IElementFinder>();

        protected override ILocalizedLogger LocalizedLogger => AqualityServices.ServiceProvider.GetRequiredService<ILocalizedLogger>();

        protected override ILocalizationManager LocalizationManager => AqualityServices.ServiceProvider.GetRequiredService<ILocalizationManager>();
    }
}
