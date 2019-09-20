using System;

namespace Aquality.Selenium.Core.Configurations
{
    /// <summary>
    /// Provides retry configuration.
    /// </summary>
    public class RetryConfiguration : IRetryConfiguration
    {
        private readonly ISettingsFile settingsFile;

        /// <summary>
        /// Instantiates class using <see cref="ISettingsFile"/>> with general settings.
        /// </summary>
        /// <param name="settingsFile">Settings file.</param>
        public RetryConfiguration(ISettingsFile settingsFile)
        {
            this.settingsFile = settingsFile;

            Number = GetIntFromSettings(nameof(Number).ToLowerInvariant());
            PollingInterval = TimeSpan.FromMilliseconds(GetIntFromSettings("pollingInterval"));
        }

        private int GetIntFromSettings(string name)
        {
            return settingsFile.GetValue<int>($".retry.{name}");
        }

        public int Number { get; }

        public TimeSpan PollingInterval { get; }
    }
}
