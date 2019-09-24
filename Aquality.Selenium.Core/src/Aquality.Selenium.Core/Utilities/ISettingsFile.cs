using System.Collections.Generic;

namespace Aquality.Selenium.Core.Configurations
{
    /// <summary>
    /// Describes reader of settings file.
    /// </summary>
    public interface ISettingsFile
    { 
        /// <summary>
        /// Gets value from settings file.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="path">Path to the value. Depending on file format, it can be jsonPath, xpath etc.</param>
        /// <returns>Value</returns>
        T GetValue<T>(string path);

        /// <summary>
        /// Gets list of values from settings file.
        /// </summary>
        /// <typeparam name="T">Type of the values.</typeparam>
        /// <param name="path">Path to the values. Depending on file format, it can be jsonPath, xpath etc.</param>
        /// <returns>List of values</returns>
        IReadOnlyList<T> GetValueList<T>(string path);

        /// <summary>
        /// Gets dictionary of keys and values from settings file.
        /// </summary>
        /// <typeparam name="T">Type of the values.</typeparam>
        /// <param name="path">Path to the values. Depending on file format, it can be jsonPath, xpath etc.</param>
        /// <returns>Dictionary of keys and values</returns>
        IReadOnlyDictionary<string, T> GetValueDictionary<T>(string path);

        /// <summary>
        /// Checks if value exists in settings.
        /// </summary>
        /// <param name="path">Path to the values. Depending on file format, it can be jsonPath, xpath etc.</param>
        /// <returns>True if exists, false otherwise.</returns>
        bool IsValuePresent(string path);
    }
}
