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
    }
}
