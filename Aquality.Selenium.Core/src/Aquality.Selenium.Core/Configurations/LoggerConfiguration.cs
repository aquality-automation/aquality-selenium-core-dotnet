﻿using Aquality.Selenium.Core.Utilities;

namespace Aquality.Selenium.Core.Configurations
{
    /// <summary>
    /// Provides logger configuration
    /// </summary>
    public class LoggerConfiguration : ILoggerConfiguration
    {
        private const string defaultLanguage = "en";
        private readonly JsonFile settingsFile;

        /// <summary>
        /// Instantiates class using JSON file with general settings.
        /// </summary>
        /// <param name="settingsFile">JSON settings file.</param>
        public LoggerConfiguration(JsonFile settingsFile)
        {
            this.settingsFile = settingsFile;
        }

        public string Language => settingsFile.GetValueOrDefault(".logger.language", defaultLanguage);
    }
}
