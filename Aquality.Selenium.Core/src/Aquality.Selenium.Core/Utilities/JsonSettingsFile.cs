using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Logging;

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

        private JsonDocument JsonDocument => JsonDocument.Parse(fileContent);

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
                            ? (T)Convert.ChangeType(envValue, type)
                            : (T)TypeDescriptor.GetConverter(type).ConvertFrom(envValue);
                    },
                    envValue, path);
            }

            var element = GetJsonElement(path);
            return DeserializeJsonElement<T>(element);
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
                return ConvertEnvVar(
                    () =>
                    {
                        return envValue.Split(',').Select(value =>
                            (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value.Trim())).ToList();
                    }, envValue, path);
            }

            var element = GetJsonElement(path);
            return DeserializeJsonElementList<T>(element);
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
            var element = GetJsonElement(path);

            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    dict.Add(property.Name, GetValue<T>($"{path}['{property.Name}']"));
                }
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
            return GetEnvironmentValue(path) != null || TryGetJsonElement(path, out _);
        }

        private static string GetEnvironmentValue(string jsonPath)
        {
            var key = jsonPath.Replace("['", ".").Replace("']", string.Empty).Substring(1);
            return EnvironmentConfiguration.GetVariable(key);
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

        private JsonElement GetJsonElement(string jsonPath)
        {
            if (!TryGetJsonElement(jsonPath, out var element))
            {
                throw new ArgumentException(
                    $"There are no values found by path '{jsonPath}' in JSON file '{resourceName}'");
            }

            return element;
        }
        
        private bool TryGetJsonElement(string jsonPath, out JsonElement targetElement)
        {
            targetElement = default;

            try
            {
                var element = JsonDocument.RootElement;
                var path = NormalizePath(jsonPath);

                if (string.IsNullOrEmpty(path))
                {
                    targetElement = element;
                    return true;
                }

                var pathParts = ParsePath(path);
                foreach (var part in pathParts)
                {
                    if (element.ValueKind != JsonValueKind.Object)
                    {
                        return false;
                    }

                    if (!element.TryGetProperty(part, out element))
                    {
                        return false;
                    }
                }

                targetElement = element;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string[] ParsePath(string jsonPath)
        {
            var pathSegments = new List<string>();
            var currentSegment = string.Empty;
            var isInBracket = false;
            
            for (var i = 0; i < jsonPath.Length; i++)
            {
                var currentChar = jsonPath[i];

                switch (currentChar)
                {
                    case '.' when !isInBracket:
                        AddSegmentIfNotEmpty(pathSegments, ref currentSegment);
                        break;

                    case '[':
                        AddSegmentIfNotEmpty(pathSegments, ref currentSegment);
                        isInBracket = true;
                        break;

                    case ']':
                        if (isInBracket)
                        {
                            AddBracketSegment(pathSegments, currentSegment);
                            currentSegment = string.Empty;
                            isInBracket = false;
                        }
                        break;

                    default:
                        currentSegment += currentChar;
                        break;
                }
            }
            
            AddSegmentIfNotEmpty(pathSegments, ref currentSegment);
            return pathSegments.ToArray();
        }
        
        private static void AddSegmentIfNotEmpty(List<string> pathSegments, ref string currentSegment)
        {
            if (!string.IsNullOrEmpty(currentSegment))
            {
                pathSegments.Add(currentSegment);
                currentSegment = string.Empty;
            }
        }
        
        private static void AddBracketSegment(List<string> pathSegments, string currentSegment)
        {
            if (currentSegment.StartsWith("'") && currentSegment.EndsWith("'"))
            {
                pathSegments.Add(currentSegment.Substring(1, currentSegment.Length - 2));
            }
            else
            {
                pathSegments.Add(currentSegment);
            }
        }
        
        private static string NormalizePath(string jsonPath)
        {
            if (jsonPath.StartsWith("."))
                jsonPath = jsonPath.Substring(1);
            
            if (jsonPath.StartsWith("$")) 
                jsonPath = jsonPath.StartsWith("$.") ? jsonPath.Substring(2) : jsonPath.Substring(1);
                
            return jsonPath;
        }
        
        private T DeserializeJsonElement<T>(JsonElement element)
        {
            if (typeof(T) == typeof(object))
            {
                return (T)DeserializeAsObject(element);
            }

            return JsonSerializer.Deserialize<T>(element.GetRawText());
        }
        
        private IReadOnlyList<T> DeserializeJsonElementList<T>(JsonElement element)
        {
            if (typeof(T) == typeof(object))
            {
                var jsonArray = element.EnumerateArray();
                var result = new List<object>();

                foreach (var item in jsonArray)
                {
                    var deserializedItem = DeserializeAsObject(item);
                    result.Add(deserializedItem);
                }

                return (IReadOnlyList<T>)result;
            }

            return JsonSerializer.Deserialize<IReadOnlyList<T>>(element.GetRawText());
        }
        
        private object DeserializeAsObject(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt32(out var intValue) ? intValue : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => JsonSerializer.Deserialize<object>(element.GetRawText())
            };
        }
    }
}