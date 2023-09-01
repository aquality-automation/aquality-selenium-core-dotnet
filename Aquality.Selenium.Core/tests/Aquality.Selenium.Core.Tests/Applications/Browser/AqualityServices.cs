using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

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
            // temp workaround to handle ChromeDriver issue: https://groups.google.com/g/chromedriver-users/c/Dgv9xRHZf58
            return services.GetService<IActionRetrier>().DoWithRetry(() =>
                new ChromeApplication(services.GetRequiredService<ITimeoutConfiguration>()), new List<Type> { typeof(InvalidOperationException) });
        }
    }
}
