using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class AqualityServices : AqualityServices<ChromeApplication>
    {
        public new static bool IsApplicationStarted => IsApplicationStarted();

        public static ChromeApplication Application => GetApplication(services => StartChrome(services));

        public static IServiceProvider ServiceProvider 
        { 
            get
            {
                return GetServiceProvider(services => Application);
            }
            set
            {
                SetServiceProvider(value);
            }
        }            

        private static ChromeApplication StartChrome(IServiceProvider services)
        {
            return new ChromeApplication(services.GetRequiredService<ITimeoutConfiguration>());
        }
    }
}
