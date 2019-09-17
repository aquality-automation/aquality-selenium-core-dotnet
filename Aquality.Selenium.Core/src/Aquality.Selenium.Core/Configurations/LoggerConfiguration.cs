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
        /// Instantiates class using JSON file with general settings.
        /// </summary>
        /// <param name="settingsFile">JSON settings file.</param>
        public LoggerConfiguration(JsonFile settingsFile)
        {
            Language = settingsFile.GetValueOrDefault(".logger.language", DefaultLanguage);
        }

        public string Language { get; }
    }
}
