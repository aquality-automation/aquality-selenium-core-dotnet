namespace Aquality.Selenium.Core.Localization
{
    /// <summary>
    /// This interface is using for translation messages to different languages
    /// </summary>
    public interface ILocalizationManager
    {
        /// <summary>
        /// Get localized message from resources by its key.
        /// </summary>
        /// <param name="messageKey">Key in resource file.</param>
        /// <param name="args">Arguments, which will be provided to template of localized message.</param>
        /// <returns>Localized message.</returns>
        string GetLocalizedMessage(string messageKey, params object[] args);
    }
}
