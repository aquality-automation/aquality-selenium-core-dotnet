using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Utilities;
using System.Reflection;

namespace Aquality.Selenium.Core.Localization
{
    /// <summary>
    /// This class is using for translation messages to different languages
    /// </summary>
    public class LocalizationManager
    {
        private const string LangResource = "Resources.Localization.{0}.json";
        private readonly JsonFile localizationFile;
        private readonly Logger logger;

        public LocalizationManager(ILoggerConfiguration loggerConfiguration, Logger logger, Assembly assembly = null)
        {
            var language = loggerConfiguration.Language;
            localizationFile = new JsonFile(string.Format(LangResource, language.ToLower()), assembly ?? Assembly.GetExecutingAssembly());
            this.logger = logger;
        }

        /// <summary>
        /// Get localized message from resources by its key.
        /// </summary>
        /// <param name="messageKey">Key in resource file.</param>
        /// <param name="args">Arguments, which will be provided to template of localized message.</param>
        /// <returns>Localized message.</returns>
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
