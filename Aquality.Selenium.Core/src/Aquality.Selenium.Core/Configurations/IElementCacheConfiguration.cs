namespace Aquality.Selenium.Core.Configurations
{
    /// <summary>
    /// Represents element's cache configuration.
    /// </summary>
    public interface IElementCacheConfiguration
    {
        /// <summary>
        /// Is element caching allowed or not.
        /// </summary>
        bool IsEnabled { get; }
    }
}
