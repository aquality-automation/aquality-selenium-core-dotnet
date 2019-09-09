using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Utilities;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Aquality.Selenium.Core.Applications
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services, Func<IServiceProvider, IApplication> applicationProvider)
        {
            var settings = GetSettings();
            services.AddScoped(applicationProvider);

            services.AddSingleton<ITimeoutConfiguration>(new TimeoutConfiguration(settings));
            services.AddTransient<ConditionalWait>();
            services.AddSingleton<ILoggerConfiguration>(new LoggerConfiguration(settings));
            services.AddSingleton(Logger.Instance);
            services.AddSingleton<LocalizationManager>();
            services.AddSingleton<LocalizationLogger>();
            services.AddSingleton<IRetryConfiguration>(new RetryConfiguration(settings));
            services.AddSingleton<ElementActionRetrier>();

            services.AddTransient<IElementFinder, ElementFinder>();
            services.AddTransient<IElementFactory, ElementFactory>();
        }

        private JsonFile GetSettings()
        {
            var profileNameFromEnvironment = EnvironmentConfiguration.GetVariable("profile");
            var settingsProfile = profileNameFromEnvironment == null ? "settings.json" : $"settings.{profileNameFromEnvironment}.json";
            Logger.Instance.Info($"Get settings from: {settingsProfile}");

            var jsonFile = FileReader.IsResourceFileExist(settingsProfile)
                ? new JsonFile(settingsProfile)
                : new JsonFile($"Resources.{settingsProfile}", Assembly.GetCallingAssembly());
            return jsonFile;
        }
    }
}
