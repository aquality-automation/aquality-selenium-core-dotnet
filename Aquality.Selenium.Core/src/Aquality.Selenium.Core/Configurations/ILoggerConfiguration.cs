namespace Aquality.Selenium.Core.Configurations
{
    /// <summary>
    /// Describes logger configuration.
    /// </summary>
    public interface ILoggerConfiguration
    {
        /// <summary>
        /// Gets language of framework.
        /// </summary>
        /// <value>Supported language.</value>
        string Language { get; }

        /// <summary>
        /// Perform page source logging in case of catastrophic failures or not.
        /// </summary>
        bool LogPageSource { get; }

        /// <summary>
        /// Do log attribute value or not.
        /// </summary>
        bool LogAttributeValue { get; }

        /// <summary>
        /// Do log text value or not.
        /// </summary>
        bool LogTextValue { get; }

        /// <summary>
        /// Do log wait for element's state or not.
        /// </summary>
        bool LogWaitForState { get; }
    }
}
