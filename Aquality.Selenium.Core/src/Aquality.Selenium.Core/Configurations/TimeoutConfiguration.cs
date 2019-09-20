using System;

namespace Aquality.Selenium.Core.Configurations
{
    /// <summary>
    /// Provides timeouts configuration.
    /// </summary>
    public class TimeoutConfiguration : ITimeoutConfiguration
    {
        private readonly ISettingsFile settingsFile;

        /// <summary>
        /// Instantiates class using <see cref="ISettingsFile"/>> with general settings.
        /// </summary>
        /// <param name="settingsFile">Settings file.</param>
        public TimeoutConfiguration(ISettingsFile settingsFile)
        {
            this.settingsFile = settingsFile;
            Implicit = GetTimeoutFromSeconds(nameof(Implicit));
            Condition = GetTimeoutFromSeconds(nameof(Condition));
            PollingInterval = TimeSpan.FromMilliseconds(GetIntFromTimeoutSettings(nameof(PollingInterval)));
            Command = GetTimeoutFromSeconds(nameof(Command));
        }

        private TimeSpan GetTimeoutFromSeconds(string name)
        {
            return TimeSpan.FromSeconds(GetIntFromTimeoutSettings(name));
        }

        private int GetIntFromTimeoutSettings(string name)
        {
            return settingsFile.GetValue<int>($".timeouts.timeout{name}");
        }

        public TimeSpan Implicit { get; }

        public TimeSpan Condition { get; }

        public TimeSpan PollingInterval { get; }

        public TimeSpan Command { get; }
    }
}
