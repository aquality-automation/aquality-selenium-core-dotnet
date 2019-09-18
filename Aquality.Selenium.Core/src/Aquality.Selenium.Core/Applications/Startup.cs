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
    /// <summary>
    /// Allows to resolve dependencies for all services in the Aquality.Selenium.Core library
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Used to configure dependencies for services of the current library
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="applicationProvider">function that provides an instance of <see cref="IApplication"/></param>
        /// <param name="settingsFile"><see cref="JsonFile"/> with settings for configuration of dependencies.
        /// Pass the result of <see cref="GetSettings"/> if you need to get settings from the embedded resource of your project.</param>
        public void ConfigureServices(IServiceCollection services, Func<IServiceProvider, IApplication> applicationProvider, ISettingsFile settingsFile = null)
        {
            var settings = settingsFile ?? GetSettings();
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

        /// <summary>
        /// Provides a <see cref="JsonFile"/> with settings.
        /// If "profile" environment variable is defined, it will be used in the name : $"settings.{profile}.json";
        /// Otherwise, will use default name of settings file: "settings.json".
        /// Will look for the resource file (copied to binaries/Resources/ folder);
        /// If not found, will look for embedded resource in the calling assembly of this method
        /// </summary>
        /// <returns>An instance of settings JsonFile</returns>
        public ISettingsFile GetSettings()
        {
            var profileNameFromEnvironment = EnvironmentConfiguration.GetVariable("profile");
            var settingsProfile = profileNameFromEnvironment == null ? "settings.json" : $"settings.{profileNameFromEnvironment}.json";
            Logger.Instance.Debug($"Get settings from: {settingsProfile}");

            var jsonFile = FileReader.IsResourceFileExist(settingsProfile)
                ? new JsonSettingsFile(settingsProfile)
                : new JsonSettingsFile($"Resources.{settingsProfile}", Assembly.GetCallingAssembly());
            return jsonFile;
        }
    }
}
