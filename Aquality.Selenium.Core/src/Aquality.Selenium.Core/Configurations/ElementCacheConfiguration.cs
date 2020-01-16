﻿namespace Aquality.Selenium.Core.Configurations
{
    /// <summary>
    /// Provides element's cache configuration
    /// </summary>
    public class ElementCacheConfiguration : IElementCacheConfiguration
    {
        /// <summary>
        /// Instantiates class using <see cref="ISettingsFile"/> with general settings.
        /// </summary>
        /// <param name="settingsFile">Settings file.</param>
        public ElementCacheConfiguration(ISettingsFile settingsFile)
        {
            var jPath = ".elementCache.isEnabled";
            IsEnabled = settingsFile.IsValuePresent(jPath) && settingsFile.GetValue<bool>(jPath);
        }

        public bool IsEnabled { get; }
    }
}
