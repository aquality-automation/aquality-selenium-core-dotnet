using Aquality.Selenium.Core.Utilities;

namespace Aquality.Selenium.Core.Configurations
{
    /// <summary>
    /// Provides logger configuration
    /// </summary>
    public class LoggerConfiguration : ILoggerConfiguration
    {
        private const string DefaultLanguage = "en";

        /// <summary>
        /// Instantiates class using <see cref="ISettingsFile"/> with general settings.
        /// </summary>
        /// <param name="settingsFile">Settings file</param>
        public LoggerConfiguration(ISettingsFile settingsFile)
        {
            Language = settingsFile.GetValueOrDefault(".logger.language", DefaultLanguage);
            LogPageSource = settingsFile.GetValueOrDefault(".logger.logPageSource", true);
            LogAttributeValue = settingsFile.GetValueOrDefault(".logger.logAttributeValue", true);
            LogTextValue = settingsFile.GetValueOrDefault(".logger.logTextValue", true);
            LogWaitForState = settingsFile.GetValueOrDefault(".logger.logWaitForState", true);
        }

        public string Language { get; }

        public bool LogPageSource { get; }

        public bool LogAttributeValue { get; }

        public bool LogTextValue { get; }

        public bool LogWaitForState { get; }
    }
}
