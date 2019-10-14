using System;

namespace Aquality.Selenium.Core.Localization
{
    /// <summary>
    /// Log messages to different languages
    /// </summary>
    public interface ILocalizationLogger
    {
        /// <summary>
        /// Logs localized message for action with INFO level which is applied for element, for example, click, send keys etc.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="messageKey">Key in resource file.</param>
        /// <param name="args">Arguments, which will be provided to template of localized message.</param>
        void InfoElementAction(string elementType, string elementName, string messageKey, params object[] args);

        /// <summary>
        /// Logs localized message with INFO level.
        /// </summary>
        /// <param name="messageKey">Key in resource file.</param>
        /// <param name="args">Arguments, which will be provided to template of localized message.</param>
        void Info(string messageKey, params object[] args);

        /// <summary>
        /// Logs localized message with DEBUG level.
        /// </summary>
        /// <param name="messageKey">Key in resource file.</param>
        /// <param name="exception">Exception, gets null value by default.</param>
        /// <param name="args">Arguments, which will be provided to template of localized message.</param>
        void Debug(string messageKey, Exception exception = null, params object[] args);

        /// <summary>
        /// Logs localized message with WARN level.
        /// </summary>
        /// <param name="messageKey">Key in resource file.</param>
        /// <param name="args">Arguments, which will be provided to template of localized message.</param>
        void Warn(string messageKey, params object[] args);

        /// <summary>
        /// Logs localized message with ERROR level.
        /// </summary>
        /// <param name="messageKey">Key in resource file.</param>
        /// <param name="args">Arguments, which will be provided to template of localized message.</param>
        void Error(string messageKey, params object[] args);

        /// <summary>
        /// Logs localized message with FATAL level.
        /// </summary>
        /// <param name="messageKey">Key in resource file.</param>
        /// <param name="exception">Exception, gets null value by default.</param>
        /// <param name="args">Arguments, which will be provided to template of localized message.</param>
        void Fatal(string messageKey, Exception exception = null, params object[] args);
    }
}
