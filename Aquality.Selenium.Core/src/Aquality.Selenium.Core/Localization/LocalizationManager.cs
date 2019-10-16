using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Utilities;
using System.Reflection;

namespace Aquality.Selenium.Core.Localization
{
    public class LocalizationManager : ILocalizationManager
    {
        private const string LangResource = "Resources.Localization.{0}.json";
        private readonly ISettingsFile localizationFile;
        private readonly Logger logger;

        public LocalizationManager(ILoggerConfiguration loggerConfiguration, Logger logger, Assembly assembly = null)
        {
            var language = loggerConfiguration.Language;
            localizationFile = new JsonSettingsFile(string.Format(LangResource, language.ToLower()), assembly ?? Assembly.GetExecutingAssembly());
            this.logger = logger;
        }

        public string GetLocalizedMessage(string messageKey, params object[] args)
        {
            var jsonKey = $"$['{messageKey}']";
            if (localizationFile.IsValuePresent(jsonKey))
            {
                return string.Format(localizationFile.GetValue<string>(jsonKey), args);
            }

            logger.Debug($"Cannot find localized message by key '{jsonKey}'");
            return messageKey;
        }
    }
}
