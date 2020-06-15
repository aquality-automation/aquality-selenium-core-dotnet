using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Utilities;
using System.Linq;
using System.Reflection;

namespace Aquality.Selenium.Core.Localization
{
    public class LocalizationManager : ILocalizationManager
    {
        private const string LangResource = "Resources.Localization.{0}.json";
        private readonly ISettingsFile localizationFile;
        private readonly ISettingsFile coreLocalizationFile;
        private readonly Logger logger;

        public LocalizationManager(ILoggerConfiguration loggerConfiguration, Logger logger, Assembly assembly = null)
        {
            var language = loggerConfiguration.Language;
            localizationFile = GetLocalizationFile(language, assembly ?? Assembly.GetExecutingAssembly());
            coreLocalizationFile = GetLocalizationFile(language, Assembly.GetExecutingAssembly());
            this.logger = logger;
        }

        private static ISettingsFile GetLocalizationFile(string language, Assembly assembly)
        {
            var embeddedResourceName = string.Format(LangResource, language.ToLower());
            var assemblyToUse = assembly.GetManifestResourceNames().Any(name => name.Contains(embeddedResourceName))
                ? assembly 
                : Assembly.GetExecutingAssembly();
            return new JsonSettingsFile(embeddedResourceName, assemblyToUse);
        }

        public string GetLocalizedMessage(string messageKey, params object[] args)
        {
            var jsonKey = $"$['{messageKey}']";
            var localizationFileToUse = localizationFile.IsValuePresent(jsonKey) ? localizationFile : coreLocalizationFile;
            if (localizationFileToUse.IsValuePresent(jsonKey))
            {
                return string.Format(localizationFileToUse.GetValue<string>(jsonKey), args);
            }

            logger.Debug($"Cannot find localized message by key '{jsonKey}'");
            return messageKey;
        }
    }
}
