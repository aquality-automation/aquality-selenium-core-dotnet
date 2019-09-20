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
        /// Instantiates class using <see cref="ISettingsFile"/>> with general settings.
        /// </summary>
        /// <param name="settingsFile">Settings file</param>
        public LoggerConfiguration(ISettingsFile settingsFile)
        {
            Language = settingsFile.GetValueOrDefault(".logger.language", DefaultLanguage);
        }

        public string Language { get; }
    }
}
