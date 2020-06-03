using System.Collections.Generic;
using Aquality.Selenium.Core.Configurations;

namespace Aquality.Selenium.Core.Utilities
{
    public static class SettingsFileExtensions
    {
        /// <summary>
        /// Gets value from environment\setting file or return default of new T() if path doesn't exist in file and environment variables.
        /// Exception will not be threw.
        /// </summary>
        /// <typeparam name="T">Type of a value.</typeparam>
        /// <param name="file">Settings file.</param>
        /// <param name="path">Path to a value. Depends on file format, it can be xpath, path etc.</param>
        /// <returns>Value or default(T).</returns>
        public static T GetValueOrNew<T>(this ISettingsFile file, string path) where T : new()
        {
            return GetValueOrDefault(file, path, new T());
        }

        /// <summary>
        /// Gets list of values from environment\setting file or empty List if path doesn't exist in file and environment variables.
        /// Exception will not be threw.
        /// </summary>
        /// <typeparam name="T">Type of a value.</typeparam>
        /// <param name="file">Settings file.</param>
        /// <param name="path">Path to a value. Depends on file format, it can be xpath, path etc.</param>
        /// <returns>List of values or empty List.</returns>
        public static IReadOnlyList<T> GetValueListOrEmpty<T>(this ISettingsFile file, string path)
        {
            return file.IsValuePresent(path) ? file.GetValueList<T>(path) : new List<T>();
        }

        /// <summary>
        /// Gets dictionary of values from environment\setting file or empty Dictionary if path doesn't exist in file and environment variables.
        /// Exception will not be thrown.
        /// Note that each value of dictionary that is present in settings file can be overriden via Environment variable with the same name;
        /// (e.g. for capability "safebrowsing.enabled" at dictionary path ".driverSettings.chrome.options" you can set environment variable "driverSettings.chrome.options.safebrowsing.enabled")
        /// </summary>
        /// <typeparam name="T">Type of a value.</typeparam>
        /// <param name="file">Settings file.</param>
        /// <param name="path">Path to a value. Depends on file format, it can be xpath, path etc.</param>
        /// <returns>Dictionary of values or empty Dictionary.</returns>
        public static IReadOnlyDictionary<string, T> GetValueDictionaryOrEmpty<T>(this ISettingsFile file, string path)
        {
            return file.IsValuePresent(path) ? file.GetValueDictionary<T>(path) : new Dictionary<string, T>();
        }

        /// <summary>
        /// Gets value from environment\setting file or return default of T if path doesn't exist in file and environment variables.
        /// Exception will not be threw.
        /// </summary>
        /// <typeparam name="T">Type of a value.</typeparam>
        /// <param name="file">Settings file.</param>
        /// <param name="path">Path to a value. Depends on file format, it can be xpath, path etc.</param>
        /// <param name="defaultValue">Default value. default(T) if not specified.</param>
        /// <returns>Value or defaultValue or default(T)</returns>
        public static T GetValueOrDefault<T>(this ISettingsFile file, string path, T defaultValue = default(T))
        {
            return file.IsValuePresent(path) ? file.GetValue<T>(path) : defaultValue;
        }
    }
}
