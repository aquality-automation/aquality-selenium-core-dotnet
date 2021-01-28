using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aquality.Selenium.Core.Utilities
{
    /// <summary>
    /// Provides methods to get info from JSON files.
    /// Note that the value can be overriden via Environment variable with the same name
    /// (e.g. for json path ".timeouts.timeoutScript" you can set environment variable "timeouts.timeoutScript"
    /// </summary>
    public class JsonSettingsFile : ISettingsFile
    {
        private readonly string fileContent;
        private readonly string resourceName;

        private JObject JsonObject => JsonConvert.DeserializeObject<JObject>(fileContent);

        /// <summary>
        /// Inistantiates class using desired JSON fileinfo.
        /// </summary>
        /// <param name="fileInfo">JSON fileinfo.</param>
        public JsonSettingsFile(FileInfo fileInfo)
        {
            resourceName = fileInfo.Name;
            fileContent = FileReader.GetTextFromFile(fileInfo);
        }

        /// <summary>
        /// Inistantiates class using desired resource file info.
        /// </summary>
        /// <param name="resourceFileName"></param>
        public JsonSettingsFile(string resourceFileName)
        {
            resourceName = resourceFileName;
            fileContent = FileReader.GetTextFromResource(resourceFileName);
        }

        /// <summary>
        /// Inistantiates class using desired embeded resource.
        /// </summary>
        /// <param name="embededResourceName">Embeded resource name</param>
        /// <param name="assembly">Assembly which resource belongs to</param>
        public JsonSettingsFile(string embededResourceName, Assembly assembly)
        {
            resourceName = embededResourceName;
            fileContent = FileReader.GetTextFromEmbeddedResource(embededResourceName, assembly);
        }

        /// <summary>
        /// Gets value from JSON.
        /// Note that the value can be overriden via Environment variable with the same name
        /// (e.g. for json path ".timeouts.timeoutScript" you can set environment variable "timeouts.timeoutScript")
        /// </summary>
        /// <param name="path">Relative JsonPath to the value.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>Value from JSON/Environment by JsonPath.</returns>
        /// <exception cref="ArgumentException">Throws when there is no value found by jsonPath in desired JSON file.</exception>
        public T GetValue<T>(string path)
        {
            var envValue = GetEnvironmentValue(path);
            if (envValue != null)
            {
                return ConvertEnvVar(() =>
                {
                    var type = typeof(T);
                    return type == typeof(object) 
                    ? (T) Convert.ChangeType(envValue, type)
                    : (T) TypeDescriptor.GetConverter(type).ConvertFrom(envValue);
                },
                    envValue, path);
            }

            var node = GetJsonNode(path);
            return node.ToObject<T>();
        }

        /// <summary>
        /// Gets list of values from JSON.
        /// Note that the value can be overriden via Environment variable with the same name; values must be separated by ','
        /// (e.g. for json path ".driverSettings.chrome.startArguments" you can set environment variable "driverSettings.chrome.startArguments")
        /// </summary>
        /// <param name="path">Relative JsonPath to the values.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>Value from JSON/Environment by JsonPath.</returns>
        /// <exception cref="ArgumentException">Throws when there are no values found by jsonPath in desired JSON file.</exception>
        public IReadOnlyList<T> GetValueList<T>(string path)
        {
            var envValue = GetEnvironmentValue(path);
            if (envValue != null)
            {
                return ConvertEnvVar(() =>
                {
                    return envValue.Split(',').Select(value => (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value.Trim())).ToList();
                }, envValue, path);
            }

            var node = GetJsonNode(path);
            return node.ToObject<IReadOnlyList<T>>();
        }

        /// <summary>
        /// Gets dictionary of values from JSON.
        /// Note that the value can be overriden via Environment variable with the same name;
        /// (e.g. for json path ".timeouts.timeoutImplicit" you can set environment variable ".timeouts.timeoutImplicit")
        /// </summary>
        /// <param name="path">Relative JsonPath to the values.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>Value from JSON/Environment by JsonPath.</returns>
        /// <exception cref="ArgumentException">Throws when there are no values found by jsonPath in desired JSON file.</exception>
        public IReadOnlyDictionary<string, T> GetValueDictionary<T>(string path)
        {
            var dict = new Dictionary<string, T>();
            var node = GetJsonNode(path);
            foreach (var child in node.Children<JProperty>())
            {
                dict.Add(child.Name, GetValue<T>($".{child.Path}"));
            }

            return dict;
        }

        /// <summary>
        /// Checks whether value present on JSON/Environment by JsonPath or not.
        /// </summary>
        /// <param name="path">Relative JsonPath to the object.</param>
        /// <returns>True if present and false otherwise.</returns>
        public bool IsValuePresent(string path)
        {
            return GetEnvironmentValue(path) != null || JsonObject.SelectToken(path) != null;
        }

        private static string GetEnvironmentValue(string jsonPath)
        {
            var key = jsonPath.Replace("['", ".").Replace("']", string.Empty).Substring(1);
            return EnvironmentConfiguration.GetVariable(key);
        }

        private JToken GetJsonNode(string jsonPath)
        {
            var node = JsonObject.SelectToken(jsonPath);
            if (node == null)
            {
                throw new ArgumentException($"There are no values found by path '{jsonPath}' in JSON file '{resourceName}'");
            }
            return node;
        }

        private static T ConvertEnvVar<T>(Func<T> convertMethod, string envValue, string jsonPath)
        {
            Logger.Instance.Debug($"***** Using variable passed from environment {jsonPath.Substring(1)}={envValue}");
            try
            {
                return convertMethod();
            }
            catch (ArgumentException ex)
            {
                var message = $"Value of '{jsonPath}' environment variable has incorrect format: {ex.Message}";
                throw new ArgumentException(message);
            }
        }
    }
}
